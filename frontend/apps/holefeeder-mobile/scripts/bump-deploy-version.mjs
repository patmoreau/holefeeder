import { execFileSync } from 'node:child_process';
import { writeFileSync } from 'node:fs';
import { DeployVersions } from './deploy-version.ts';

const FILE = 'deploy-version.json';

const git = (...args) => execFileSync('git', args, { encoding: 'utf8', stdio: ['ignore', 'pipe', 'ignore'] }).trim();

const lastDeployed = () => {
  try {
    return JSON.parse(git('show', `HEAD:./${FILE}`));
  } catch {
    return undefined;
  }
};

const dirty = git('status', '--porcelain')
  .split('\n')
  .some((line) => line.trim() !== '' && !line.endsWith(FILE));
const commit = git('rev-parse', '--short', 'HEAD') + (dirty ? '-dirty' : '');

const next = DeployVersions.next(lastDeployed(), new Date(), commit);
writeFileSync(FILE, `${JSON.stringify(next, null, 2)}\n`);
console.log(`Deploying ${next.version} (${next.build}) from ${next.commit}`);
