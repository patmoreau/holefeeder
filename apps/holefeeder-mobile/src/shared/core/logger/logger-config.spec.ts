import { LoggerConfig } from '@/shared/core/logger/logger-config';

describe('LoggerConfig', () => {
  const originalForceLogs = process.env.EXPO_PUBLIC_FORCE_LOGS;

  afterEach(() => {
    const restoreEnv = (env: string, originalValue?: string) => {
      if (originalValue) {
        process.env[env] = originalValue;
      } else {
        delete process.env[env];
      }
    };

    restoreEnv('EXPO_PUBLIC_FORCE_LOGS', originalForceLogs);
  });

  it('returns true when EXPO_PUBLIC_FORCE_LOGS is set to "true"', () => {
    process.env.EXPO_PUBLIC_FORCE_LOGS = 'true';
    const loggerConfig = LoggerConfig.parseEnv();

    expect(loggerConfig).toBeSuccessWithValue({ loggingEnabled: true });
  });

  it('returns false when EXPO_PUBLIC_FORCE_LOGS is not set to "true"', () => {
    process.env.EXPO_PUBLIC_FORCE_LOGS = 'not true';
    const loggerConfig = LoggerConfig.parseEnv();

    expect(loggerConfig).toBeSuccessWithValue({ loggingEnabled: false });
  });
});
