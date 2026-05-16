import { OPSqliteOpenFactory } from '@powersync/op-sqlite';
import { PowerSyncDatabase } from '@powersync/react-native';
import { AppSchema } from '@/shared/persistence/app-schema';

let powerSync: PowerSyncDatabase | null = null;

const setupDatabase = async (): Promise<PowerSyncDatabase> => {
  if (powerSync) return powerSync;

  const factory = new OPSqliteOpenFactory({
    dbFilename: 'holefeeder.db',
  });

  powerSync = new PowerSyncDatabase({
    schema: AppSchema,
    database: factory,
  });

  await powerSync.init();
  return powerSync;
};

export const DatabaseFactory = {
  init: setupDatabase,
  instance: () => powerSync,
};
