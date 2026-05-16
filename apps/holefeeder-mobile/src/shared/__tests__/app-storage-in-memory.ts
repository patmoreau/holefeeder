import { AppStorage } from '@/shared/persistence/app-storage';

export type AppStorageInMemory = AppStorage & {
  simulateError(error: Error): void;
};

export const AppStorageInMemory = (): AppStorageInMemory => {
  const storage = new Map<string, string>();
  let simulatedError: Error | undefined = undefined;

  return {
    setString: (key: string, value: string) => {
      if (simulatedError) {
        throw simulatedError;
      }
      storage.set(key, value);
    },
    getString: (key: string) => {
      if (simulatedError) {
        throw simulatedError;
      }
      return storage.get(key);
    },
    simulateError: (error: Error) => {
      simulatedError = error;
    },
  };
};
