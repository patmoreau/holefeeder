export type AppVersionConfig = {
  version?: string;
  ios?: { buildNumber?: string };
  extra?: { deployCommit?: unknown };
} | null;

const label = (config: AppVersionConfig | undefined): string => {
  if (!config?.version) return 'unknown';
  const build = config.ios?.buildNumber;
  const commit = typeof config.extra?.deployCommit === 'string' ? config.extra.deployCommit : undefined;
  const withBuild = build ? `${config.version} (${build})` : config.version;
  return commit ? `${withBuild} · ${commit}` : withBuild;
};

export const AppVersion = {
  label: label,
};
