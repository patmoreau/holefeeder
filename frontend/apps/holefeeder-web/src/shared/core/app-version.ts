export type AppVersionInfo = {
  version: string | undefined;
  commit: string | undefined;
};

const present = (value: string | undefined): string | undefined => (value?.trim() ? value.trim() : undefined);

const label = ({ version, commit }: AppVersionInfo): string => {
  const shortCommit = present(commit)?.slice(0, 8);
  const name = present(version) ?? 'dev';
  return shortCommit ? `${name} · ${shortCommit}` : name;
};

export const AppVersion = {
  label: label,
};
