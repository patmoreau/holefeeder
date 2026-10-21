import { AppVersion } from '@/settings/core/app-version';

describe('AppVersion.label', () => {
  it('shows the version, the build and the commit the app was deployed from', () => {
    const label = AppVersion.label({ version: '2026.10.20', ios: { buildNumber: '2' }, extra: { deployCommit: '650039e9' } });

    expect(label).toBe('2026.10.20 (2) · 650039e9');
  });

  it('leaves the commit out when the build does not carry one', () => {
    const label = AppVersion.label({ version: '2026.10.20', ios: { buildNumber: '2' }, extra: {} });

    expect(label).toBe('2026.10.20 (2)');
  });

  it('shows only the version when the build number is missing', () => {
    const label = AppVersion.label({ version: '2026.10.20' });

    expect(label).toBe('2026.10.20');
  });

  it('reports an unknown version when the app has no config', () => {
    expect(AppVersion.label(null)).toBe('unknown');
  });
});
