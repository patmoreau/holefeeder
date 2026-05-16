import { Result } from '@holefeeder/core';
import { EnvVariable } from '@/shared/core/env/env-variable';

export type LoggerConfig = {
  loggingEnabled: boolean;
};

const parseEnv = (): Result<LoggerConfig> => {
  const forceLogs = EnvVariable.read('EXPO_PUBLIC_FORCE_LOGS');

  if (forceLogs.isFailure) return forceLogs;

  return Result.success({ loggingEnabled: forceLogs.value.toLowerCase() === 'true' });
};

export const LoggerConfig = {
  parseEnv: parseEnv,
} as const;
