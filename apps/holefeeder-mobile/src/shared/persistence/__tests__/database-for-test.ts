import fs from 'fs';
import {
  AbstractPowerSyncDatabase,
  type CreateSyncImplementationOptions,
  type DBAdapter,
  type PowerSyncBackendConnector,
  type PowerSyncDatabaseOptionsWithSettings,
  type RequiredAdditionalConnectionOptions,
  Schema,
  SqliteBucketStorage,
  type StreamingSyncImplementation,
} from '@powersync/common';
import { DatabaseAdapterForTest } from '@/shared/persistence/__tests__/database-adapter-for-test';
import { AppSchema } from '@/shared/persistence/app-schema';

export class DatabaseForTest extends AbstractPowerSyncDatabase {
  async _initialize(): Promise<void> {
    return Promise.resolve();
  }

  protected openDBAdapter(options: PowerSyncDatabaseOptionsWithSettings): DBAdapter {
    throw new Error('openDBAdapter not supported in TestDB');
  }

  protected generateBucketStorageAdapter(): SqliteBucketStorage {
    return new SqliteBucketStorage(this.database, this.logger);
  }

  protected generateSyncStreamImplementation(
    connector: PowerSyncBackendConnector,
    options: CreateSyncImplementationOptions & RequiredAdditionalConnectionOptions
  ): StreamingSyncImplementation {
    return {
      connected: false,
      lastSyncedAt: new Date(),
      retryDelayMs: 0,
      waitForConnected: async () => {},
      triggerUpdate: async () => {},
      disconnect: async () => {},
      dispose: async () => {},
      obtainLock: async () => () => {},
    } as unknown as StreamingSyncImplementation;
  }

  async connect() {
    return;
  }

  async cleanupTestDb() {
    try {
      // db.close() disposes TriggerManagerImpl (clearing its 120s setTimeout)
      // and closes the underlying SQLite connection — prevents Jest worker leaks.
      await this.close({ disconnect: false });
    } catch {}
  }
}

export const setupDatabaseForTest = async (inMemory: boolean = true) => {
  try {
    if (!inMemory && fs.existsSync('.debug.sqlite')) {
      fs.unlinkSync('.debug.sqlite');
    }
  } catch {}
  const adapter = new DatabaseAdapterForTest(inMemory ? ':memory:' : '.debug.sqlite');

  const db = new DatabaseForTest({
    schema: AppSchema,
    database: adapter,
  });

  await db.init();

  const sqlStatements = generateSqlFromSchema(AppSchema);

  for (const sql of sqlStatements) {
    await adapter.execute(sql);
  }

  return db;
};

const generateSqlFromSchema = (schema: Schema): string[] => {
  const tables: string[] = [];
  const indexes: string[] = [];

  for (const table of schema.tables) {
    const columnDefs = table.columns.map((column) => {
      return `"${column.name}" ${column.type}`;
    });

    columnDefs.unshift('"id" TEXT PRIMARY KEY');

    tables.push(`CREATE TABLE IF NOT EXISTS "${table.name}" (${columnDefs.join(', ')});`);

    if (table.indexes) {
      for (const index of table.indexes) {
        const columns = index.columns;
        const formattedCols = columns.map((c) => `"${c.name}"`).join(', ');

        indexes.push(`CREATE INDEX IF NOT EXISTS "${index.name}" ON "${table.name}" (${formattedCols});`);
      }
    }
  }

  return [...tables, ...indexes];
};
