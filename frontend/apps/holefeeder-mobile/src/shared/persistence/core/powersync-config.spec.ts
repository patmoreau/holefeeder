import { PowersyncConfig } from '@/shared/persistence/core/powersync-config';

describe('PowersyncConfig', () => {
  const originalUrl = process.env.EXPO_PUBLIC_POWERSYNC_URL;
  const url = 'expo-public-auth0-domain';

  beforeEach(() => {
    process.env.EXPO_PUBLIC_POWERSYNC_URL = url;
  });

  afterEach(() => {
    const restoreEnv = (env: string, originalValue?: string) => {
      if (originalValue) {
        process.env[env] = originalValue;
      } else {
        delete process.env[env];
      }
    };

    restoreEnv('EXPO_PUBLIC_POWERSYNC_URL', originalUrl);
  });

  it('succeeds with env values', () => {
    const config = PowersyncConfig.parseEnv();

    expect(config).toBeSuccessWithValue({
      url: url,
    });
  });

  it.each(['EXPO_PUBLIC_POWERSYNC_URL'])('fails when %s is not set', (envVar: string) => {
    delete process.env[envVar];
    const config = PowersyncConfig.parseEnv();

    expect(config).toBeFailureWithErrors([`env-variable-${envVar}-not-found`]);
  });
});
