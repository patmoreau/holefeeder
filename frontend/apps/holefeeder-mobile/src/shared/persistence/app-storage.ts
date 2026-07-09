export type AppStorage = {
  setString(key: string, value: string): void;
  getString(key: string): string | undefined;
};
