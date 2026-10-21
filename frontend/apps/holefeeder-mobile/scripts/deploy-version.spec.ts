import { DeployVersions } from './deploy-version';

const at = (year: number, month: number, day: number, hour = 12) => new Date(year, month - 1, day, hour);

describe('DeployVersions.next', () => {
  it('starts the day at build 1 when nothing was deployed before', () => {
    const next = DeployVersions.next(undefined, at(2026, 10, 20), 'abc1234');

    expect(next).toMatchObject({ version: '2026.10.20', build: 1 });
  });

  it('counts up the build for another deploy on the same day', () => {
    const previous = DeployVersions.next(undefined, at(2026, 10, 20, 9), 'abc1234');

    const next = DeployVersions.next(previous, at(2026, 10, 20, 17), 'def5678');

    expect(next).toMatchObject({ version: '2026.10.20', build: 2 });
  });

  it('starts again at build 1 on a new day', () => {
    const previous = { ...DeployVersions.next(undefined, at(2026, 10, 20), 'abc1234'), build: 3 };

    const next = DeployVersions.next(previous, at(2026, 10, 21), 'def5678');

    expect(next).toMatchObject({ version: '2026.10.21', build: 1 });
  });

  it('writes the date without leading zeros', () => {
    const next = DeployVersions.next(undefined, at(2026, 1, 5), 'abc1234');

    expect(next.version).toBe('2026.1.5');
  });

  it('records the commit and the time of the deploy', () => {
    const now = at(2026, 10, 20);

    const next = DeployVersions.next(undefined, now, 'abc1234');

    expect(next).toMatchObject({ commit: 'abc1234', deployedAt: now.toISOString() });
  });
});
