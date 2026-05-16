import { Result } from '@holefeeder/core';
import { EnvVariable } from '@/shared/core/env/env-variable';

export type PowersyncConfig = {
  url: string;
};

const parseEnv = (): Result<PowersyncConfig> => {
  return Result.combine({
    url: EnvVariable.read('EXPO_PUBLIC_POWERSYNC_URL'),
  });
};

export const PowersyncConfig = {
  parseEnv: parseEnv,
} as const;
