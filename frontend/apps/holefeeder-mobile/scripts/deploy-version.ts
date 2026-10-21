export type DeployVersion = {
  version: string;
  build: number;
  commit: string;
  deployedAt: string;
};

const versionFor = (date: Date): string => `${date.getFullYear()}.${date.getMonth() + 1}.${date.getDate()}`;

const next = (previous: DeployVersion | undefined, now: Date, commit: string): DeployVersion => {
  const version = versionFor(now);
  return {
    version: version,
    build: previous?.version === version ? previous.build + 1 : 1,
    commit: commit,
    deployedAt: now.toISOString(),
  };
};

export const DeployVersions = {
  next: next,
};
