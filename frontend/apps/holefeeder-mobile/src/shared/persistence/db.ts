import { PowerSyncDatabase } from '@powersync/react-native';
import { AppSchema } from '@/shared/persistence/app-schema';

let powerSync: PowerSyncDatabase | null = null;

const setupDatabase = async (): Promise<PowerSyncDatabase> => {
  if (powerSync) return powerSync;

  powerSync = new PowerSyncDatabase({
    schema: AppSchema,
    database: {
      dbFilename: 'holefeeder.db',
    },
  });

  await powerSync.init();
  return powerSync;
};

export const DatabaseFactory = {
  init: setupDatabase,
  instance: () => powerSync,
};
