import { EventEmitter } from 'events';
import { Schema } from '@powersync/common';
import { PowerSyncDatabase } from '@powersync/node';
import Database from 'better-sqlite3';

// Define the connector interface based on PowerSync requirements
interface PowerSyncBackendConnector {
  fetchCredentials(): Promise<{
    endpoint: string;
    token: string;
    expiresAt?: number;
  } | null>;
  uploadData(database: any): Promise<void>;
}

interface SyncConfig {
  powerSyncUrl: string;
  powerSyncToken?: string;
  sqliteDbPath: string;
  localDbPath?: string;
  tables: string[];
  syncIntervalMs?: number;
  schema: Schema;
}

interface SyncStats {
  lastSyncTime: Date | null;
  recordsSynced: number;
  errors: number;
}

// Simple connector that returns a static token
class SimpleConnector implements PowerSyncBackendConnector {
  constructor(
    private powerSyncUrl: string,
    private token?: string
  ) {}

  async fetchCredentials() {
    if (!this.token) {
      throw new Error('PowerSync token is required');
    }

    return {
      endpoint: this.powerSyncUrl,
      token: this.token,
      expiresAt: undefined,
    };
  }

  async uploadData(database: any): Promise<void> {
    // For read-only sync, we don't need to upload changes
    // If you need to sync changes back, implement this method
    const batch = await database.getCrudBatch();
    if (!batch) {
      return;
    }

    // Mark batch as complete without uploading
    await batch.complete();
  }
}

class PowerSyncToSQLiteSync extends EventEmitter {
  private powerSyncDb: PowerSyncDatabase;
  private sqliteDb: Database.Database;
  private config: SyncConfig;
  private connector: SimpleConnector;
  private syncInterval: NodeJS.Timeout | null = null;
  private stats: SyncStats = {
    lastSyncTime: null,
    recordsSynced: 0,
    errors: 0,
  };

  constructor(config: SyncConfig) {
    super();
    this.config = config;
    this.connector = new SimpleConnector(config.powerSyncUrl, config.powerSyncToken);

    // Initialize PowerSync with schema only
    this.powerSyncDb = new PowerSyncDatabase({
      schema: config.schema,
      database: {
        dbFilename: config.localDbPath || '/data/powersync-cache.db',
      },
    });

    this.sqliteDb = new Database(config.sqliteDbPath);
  }

  async initialize(): Promise<void> {
    try {
      await this.powerSyncDb.connect(this.connector);
      this.emit('initialized');
    } catch (error) {
      this.emit('error', error);
      throw error;
    }
  }

  private async syncTable(tableName: string): Promise<number> {
    try {
      // Get all rows from PowerSync table
      const rows = await this.powerSyncDb.getAll<Record<string, any>>(`SELECT * FROM ${tableName}`);

      if (rows.length === 0) {
        return 0;
      }

      // Get column names from first row
      const columns = Object.keys(rows[0]);
      const placeholders = columns.map(() => '?').join(', ');
      const columnNames = columns.join(', ');

      // Create table if it doesn't exist (dynamic schema)
      this.createTableIfNotExists(tableName, columns);

      // Begin transaction for batch insert
      const insertStmt = this.sqliteDb.prepare(`REPLACE INTO ${tableName} (${columnNames}) VALUES (${placeholders})`);

      const insertMany = this.sqliteDb.transaction((rows: any[]) => {
        for (const row of rows) {
          const values = columns.map((col) => row[col]);
          insertStmt.run(values);
        }
      });

      insertMany(rows);
      return rows.length;
    } catch (error) {
      this.stats.errors++;
      this.emit('error', { table: tableName, error });
      throw error;
    }
  }

  private createTableIfNotExists(tableName: string, columns: string[]): void {
    // Check if table exists
    const tableExists = this.sqliteDb.prepare(`SELECT name FROM sqlite_master WHERE type='table' AND name=?`).get(tableName);

    if (!tableExists) {
      // Create table with dynamic columns (all as TEXT for simplicity)
      const columnDefs = columns.map((col) => `${col} TEXT`).join(', ');
      this.sqliteDb.exec(`CREATE TABLE ${tableName} (${columnDefs})`);
      this.emit('tableCreated', tableName);
    }
  }

  async syncAll(): Promise<void> {
    const startTime = Date.now();
    let totalRecords = 0;

    try {
      this.emit('syncStart');

      for (const table of this.config.tables) {
        const recordCount = await this.syncTable(table);
        totalRecords += recordCount;
        this.emit('tableSynced', { table, records: recordCount });
      }

      this.stats.lastSyncTime = new Date();
      this.stats.recordsSynced += totalRecords;

      const duration = Date.now() - startTime;
      this.emit('syncComplete', {
        duration,
        records: totalRecords,
        tables: this.config.tables.length,
      });
    } catch (error) {
      this.emit('syncError', error);
      throw error;
    }
  }

  startAutoSync(): void {
    const interval = this.config.syncIntervalMs || 30000; // Default 30 seconds

    if (this.syncInterval) {
      this.stopAutoSync();
    }

    this.syncInterval = setInterval(() => {
      this.syncAll().catch((error) => {
        this.emit('error', error);
      });
    }, interval) as unknown as NodeJS.Timeout;

    this.emit('autoSyncStarted', { interval });
  }

  stopAutoSync(): void {
    if (this.syncInterval) {
      clearInterval(this.syncInterval);
      this.syncInterval = null;
      this.emit('autoSyncStopped');
    }
  }

  getStats(): SyncStats {
    return { ...this.stats };
  }

  async waitForReady(): Promise<void> {
    // Wait for PowerSync to complete initial sync
    await this.powerSyncDb.waitForReady();
    this.emit('powerSyncReady');
  }

  async close(): Promise<void> {
    this.stopAutoSync();
    await this.powerSyncDb.disconnectAndClear();
    this.sqliteDb.close();
    this.emit('closed');
  }
}

// Example usage
async function main() {
  // Import schema (adjust path as needed)
  // @ts-ignore
  // eslint-disable-next-line import/no-unresolved,import-x/no-unresolved
  const { AppSchema } = await import('./app-schema.js');

  // Validate required environment variables
  if (!process.env.POWERSYNC_URL) {
    throw new Error('POWERSYNC_URL environment variable is required');
  }

  const sync = new PowerSyncToSQLiteSync({
    powerSyncUrl: process.env.POWERSYNC_URL,
    powerSyncToken: process.env.POWERSYNC_TOKEN,
    schema: AppSchema as any,
    sqliteDbPath: process.env.SQLITE_DB_PATH || '/data/local.db',
    localDbPath: process.env.POWERSYNC_CACHE_PATH || '/data/powersync-cache.db',
    tables: (process.env.SYNC_TABLES || 'accounts,cashflows,categories,store_items,transactions').split(','),
    syncIntervalMs: parseInt(process.env.SYNC_INTERVAL_MS || '60000'),
  });

  // Event listeners
  sync.on('initialized', () => {
    console.log('✓ Connected to PowerSync');
  });

  sync.on('powerSyncReady', () => {
    console.log('✓ PowerSync initial sync complete');
  });

  sync.on('syncStart', () => {
    console.log('→ Starting sync to local SQLite...');
  });

  sync.on('tableSynced', ({ table, records }) => {
    console.log(`  ✓ Synced ${records} records from ${table}`);
  });

  sync.on('syncComplete', ({ duration, records, tables }) => {
    console.log(`✓ Sync complete: ${records} records from ${tables} tables in ${duration}ms`);
  });

  sync.on('error', (error) => {
    console.error('✗ Error:', error);
  });

  try {
    console.log('Initializing PowerSync connection...');
    await sync.initialize();

    console.log('Waiting for initial sync from PowerSync server...');
    await sync.waitForReady();

    console.log('Syncing data to local SQLite...');
    await sync.syncAll();

    // Start automatic syncing
    sync.startAutoSync();

    // Display stats
    console.log('\nSync Stats:', sync.getStats());
    console.log('\nAuto-sync enabled. Press Ctrl+C to stop.');

    // Keep running for auto-sync
    process.on('SIGINT', async () => {
      console.log('\nShutting down...');
      await sync.close();
      process.exit(0);
    });

    process.on('SIGTERM', async () => {
      console.log('\nShutting down...');
      await sync.close();
      process.exit(0);
    });
  } catch (error) {
    console.error('Fatal error:', error);
    process.exit(1);
  }
}

// Run if executed directly (ES module check)
if (import.meta.url === `file://${process.argv[1]}`) {
  main();
}

export { PowerSyncToSQLiteSync, SyncConfig, SyncStats };
