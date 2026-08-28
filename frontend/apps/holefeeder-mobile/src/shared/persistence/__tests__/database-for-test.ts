import fs from 'fs';
import { type CommonPowerSyncDatabase, Schema } from '@powersync/common';
import { PowerSyncDatabase } from '@powersync/react-native';
import { DatabaseAdapterForTest } from '@/shared/persistence/__tests__/database-adapter-for-test';
import { AppSchema } from '@/shared/persistence/app-schema';

export type DatabaseForTest = CommonPowerSyncDatabase & {
  cleanupTestDb: () => Promise<void>;
};

export const setupDatabaseForTest = async (inMemory: boolean = true): Promise<DatabaseForTest> => {
  try {
    if (!inMemory && fs.existsSync('.debug.sqlite')) {
      fs.unlinkSync('.debug.sqlite');
    }
  } catch {}
  const adapter = new DatabaseAdapterForTest(inMemory ? ':memory:' : '.debug.sqlite');

  const db = new PowerSyncDatabase({
    schema: AppSchema,
    opened: adapter,
  });

  await db.init();

  const sqlStatements = generateSqlFromSchema(AppSchema);

  for (const sql of sqlStatements) {
    await adapter.execute(sql);
  }

  return Object.assign(db, {
    cleanupTestDb: async () => {
      try {
        // db.close() disposes TriggerManagerImpl (clearing its 120s setTimeout)
        // and closes the underlying SQLite connection — prevents Jest worker leaks.
        await db.close({ disconnect: false });
      } catch {}
    },
  });
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
