export type AppVersionInfo = {
  version: string | undefined;
  build?: string | undefined;
  commit: string | undefined;
};

const present = (value: string | undefined): string | undefined => (value?.trim() ? value.trim() : undefined);

const label = ({ version, build, commit }: AppVersionInfo): string => {
  const shortCommit = present(commit)?.slice(0, 8);
  const shownBuild = present(build);
  const name = present(version) ?? 'dev';
  const withBuild = shownBuild ? `${name} (${shownBuild})` : name;
  return shortCommit ? `${withBuild} · ${shortCommit}` : withBuild;
};

export const AppVersion = {
  label: label,
};
