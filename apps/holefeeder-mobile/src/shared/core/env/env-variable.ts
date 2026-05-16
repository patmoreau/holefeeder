import { default as Constants } from 'expo-constants';
import { Result } from '@/shared/core/result';

const extra: Record<string, string | undefined> = Constants.expoConfig?.extra ?? {};

export const readEnvVariable = (key: string): Result<string> => {
  // Primary: values baked in at build time via app.config.ts extra
  // Fallback: process.env for test environments (Jest)
  const value = extra[key] ?? process.env[key];
  if (value === undefined || value === '') return Result.failure([`env-variable-${key}-not-found`]);
  return Result.success(value);
};

export const EnvVariable = { read: readEnvVariable };
