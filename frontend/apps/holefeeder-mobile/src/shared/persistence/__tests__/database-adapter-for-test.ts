import { DBAdapter, LockContext, type RawQueryResult, type SqliteValue } from '@powersync/common';
import sqliteDatabase from 'better-sqlite3';

// The base LockContext builds execute/getAll/get/executeBatch on top of executeRaw, so a
// lock callback must receive a context that is distinct from the adapter itself — the
// adapter's own methods delegate back into readLock/writeLock.
class ConnectionForTest extends LockContext {
  constructor(private db: sqliteDatabase.Database) {
    super();
  }

  async executeRaw(query: string, params: unknown[] = []): Promise<RawQueryResult> {
    const stmt = this.db.prepare(query);

    if (stmt.reader) {
      const columnNames = stmt.columns().map((column) => column.name);
      const rawRows = stmt.raw().all(...params) as SqliteValue[][];
      return { columnNames, rawRows };
    }

    const info = stmt.run(...params);
    return {
      columnNames: [],
      rawRows: [],
      insertId: Number(info.lastInsertRowid),
      rowsAffected: info.changes,
    };
  }
}

export class DatabaseAdapterForTest extends DBAdapter {
  private db: sqliteDatabase.Database;
  private connection: ConnectionForTest;

  constructor(dbFilename: string) {
    super();
    this.db = new sqliteDatabase(dbFilename);
    this.connection = new ConnectionForTest(this.db);

    this.db.function('powersync_rs_version', () => '0.5.2');
    this.db.function('powersync_connection_name', () => 'test-connection');
    this.db.function('powersync_replace_schema', { varargs: true }, (..._args: unknown[]) => null);
    this.db.function('powersync_offline_sync_status', () =>
      JSON.stringify({
        connected: false,
        connecting: false,
        priority_status: [],
        downloading: null,
        streams: [],
      })
    );

    this.db.function('powersync_diff', { varargs: true }, (..._args: unknown[]) => null);
    this.db.function('powersync_validate_checkpoint', { varargs: true }, (..._args: unknown[]) => 1);
    this.db.function('powersync_clear', { varargs: true }, (..._args: unknown[]) => null);
  }

  get name(): string {
    return 'database-adapter-for-test';
  }

  get dbConnection() {
    return this.db;
  }

  async readLock<T>(fn: (tx: LockContext) => Promise<T>): Promise<T> {
    return fn(this.connection);
  }

  async writeLock<T>(fn: (tx: LockContext) => Promise<T>): Promise<T> {
    return fn(this.connection);
  }

  async refreshSchema(): Promise<void> {}

  close(): void {
    this.db.close();
  }
}
