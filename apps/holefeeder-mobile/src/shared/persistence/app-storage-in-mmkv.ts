import { createMMKV } from 'react-native-mmkv';
import { AppStorage } from '@/shared/persistence/app-storage';

const storage = createMMKV();

export const AppStorageInMmkv = (): AppStorage => {
  return {
    setString: (key: string, value: string) => storage.set(key, value),
    getString: (key: string) => storage.getString(key),
  };
};
