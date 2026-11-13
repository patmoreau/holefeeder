import { AppVersion } from '@holefeeder/shared/core';

export type AppVersionConfig = {
  version?: string;
  ios?: { buildNumber?: string };
  extra?: { deployCommit?: unknown };
} | null;

const label = (config: AppVersionConfig | undefined): string =>
  AppVersion.label({
    version: config?.version,
    build: config?.ios?.buildNumber,
    commit: typeof config?.extra?.deployCommit === 'string' ? config.extra.deployCommit : undefined,
  });

export const ExpoAppVersion = {
  label: label,
};
