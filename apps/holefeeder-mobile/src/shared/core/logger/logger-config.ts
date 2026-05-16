import { EnvVariable } from '@/shared/core/env/env-variable';
import { Result } from '@/shared/core/result';

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
