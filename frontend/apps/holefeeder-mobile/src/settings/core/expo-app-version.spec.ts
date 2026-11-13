import { ExpoAppVersion } from '@/settings/core/expo-app-version';

describe('ExpoAppVersion.label', () => {
  it('shows the version, the build and the commit the app was deployed from', () => {
    const label = ExpoAppVersion.label({ version: '2026.10.20', ios: { buildNumber: '2' }, extra: { deployCommit: '650039e9' } });

    expect(label).toBe('2026.10.20 (2) · 650039e9');
  });

  it('shortens a full commit hash to eight characters', () => {
    const label = ExpoAppVersion.label({
      version: '2026.10.20',
      ios: { buildNumber: '2' },
      extra: { deployCommit: '650039e9c0ffee1234567890' },
    });

    expect(label).toBe('2026.10.20 (2) · 650039e9');
  });

  it('leaves the commit out when the build does not carry one', () => {
    const label = ExpoAppVersion.label({ version: '2026.10.20', ios: { buildNumber: '2' }, extra: {} });

    expect(label).toBe('2026.10.20 (2)');
  });

  it('shows only the version when the build number is missing', () => {
    const label = ExpoAppVersion.label({ version: '2026.10.20' });

    expect(label).toBe('2026.10.20');
  });

  it('reports a dev version when the app has no config', () => {
    expect(ExpoAppVersion.label(null)).toBe('dev');
  });
});
