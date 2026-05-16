import { BaseObserver, type DBAdapter, type QueryResult } from '@powersync/common';
import sqliteDatabase from 'better-sqlite3';

export class DatabaseAdapterForTest extends BaseObserver<any> implements DBAdapter {
  name = 'database-adapter-for-test';
  private db: sqliteDatabase.Database;

  constructor(dbFilename: string) {
    super();
    this.db = new sqliteDatabase(dbFilename);

    this.db.function('powersync_rs_version', () => '0.4.10');
    this.db.function('powersync_connection_name', () => 'test-connection');
    this.db.function('powersync_replace_schema', { varargs: true }, (..._args: any[]) => null);
    this.db.function('powersync_offline_sync_status', () =>
      JSON.stringify({
        connected: false,
        connecting: false,
        priority_status: [],
        downloading: null,
        streams: [],
      })
    );

    this.db.function('powersync_diff', { varargs: true }, (..._args: any[]) => null);
    this.db.function('powersync_validate_checkpoint', { varargs: true }, (..._args: any[]) => 1);
    this.db.function('powersync_clear', { varargs: true }, (..._args: any[]) => null);
  }

  get dbConnection() {
    return this.db;
  }

  async getAll<T>(sql: string, parameters: any[] = []): Promise<T[]> {
    const stmt = this.db.prepare(sql);
    return stmt.all(...parameters) as T[];
  }

  async getOptional<T>(sql: string, parameters: any[] = []): Promise<T | null> {
    const stmt = this.db.prepare(sql);
    const result = stmt.get(...parameters);
    return (result as T) || null;
  }

  async get<T>(sql: string, parameters: any[] = []): Promise<T> {
    const result = await this.getOptional<T>(sql, parameters);
    if (result === null) {
      throw new Error('Query returned no results');
    }
    return result;
  }

  async execute(query: string, params: any[] = []): Promise<QueryResult> {
    const stmt = this.db.prepare(query);

    if (stmt.reader) {
      const rows = stmt.all(...params);
      return {
        rowsAffected: 0,
        rows: {
          _array: rows,
          length: rows.length,
          item: (idx: number) => rows[idx],
        },
      };
    } else {
      const info = stmt.run(...params);
      return {
        insertId: Number(info.lastInsertRowid),
        rowsAffected: info.changes,
        rows: {
          _array: [],
          length: 0,
          item: () => undefined,
        },
      };
    }
  }

  async executeRaw(query: string, params: any[] = []): Promise<any[][]> {
    const stmt = this.db.prepare(query);
    const rows = stmt.all(...params) as any[];
    return rows.map((row) => Object.values(row));
  }

  async executeBatch(query: string, params: any[][] = []): Promise<QueryResult> {
    const stmt = this.db.prepare(query);
    let totalChanges = 0;

    for (const paramSet of params) {
      const info = stmt.run(...paramSet);
      totalChanges += info.changes;
    }

    return {
      rowsAffected: totalChanges,
      rows: {
        _array: [],
        length: 0,
        item: () => undefined,
      },
    };
  }

  async readLock<T>(fn: (tx: any) => Promise<T>): Promise<T> {
    return fn(this);
  }

  async readTransaction<T>(fn: (tx: any) => Promise<T>): Promise<T> {
    return fn(this as any);
  }

  async writeLock<T>(fn: (tx: any) => Promise<T>): Promise<T> {
    return fn(this);
  }

  async writeTransaction<T>(fn: (tx: any) => Promise<T>): Promise<T> {
    return fn(this as any);
  }

  async refreshSchema(): Promise<void> {}

  async commit(): Promise<void> {}

  async rollback(): Promise<void> {}

  close(): void {
    this.db.close();
  }
}
