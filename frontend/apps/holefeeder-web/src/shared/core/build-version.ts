import { AppVersion } from './app-version';

export const buildVersionLabel = AppVersion.label({
  version: import.meta.env.VITE_APP_VERSION as string | undefined,
  commit: import.meta.env.VITE_APP_COMMIT as string | undefined,
});
