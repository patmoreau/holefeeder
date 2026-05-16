import { ApiConfig } from '@/shared/api/api-config';
import { AuthConfig } from '@/shared/auth/core/auth-config';
import { LoggerConfig } from '@/shared/core/logger/logger-config';
import { Result } from '@/shared/core/result';
import { PowersyncConfig } from '@/shared/persistence/core/powersync-config';

export type HolefeederConfig = {
  apiConfig: ApiConfig;
  authConfig: AuthConfig;
  loggerConfig: LoggerConfig;
  powersyncConfig: PowersyncConfig;
};

const parseEnv = (): Result<HolefeederConfig> =>
  Result.combine({
    apiConfig: ApiConfig.parseEnv(),
    authConfig: AuthConfig.parseEnv(),
    loggerConfig: LoggerConfig.parseEnv(),
    powersyncConfig: PowersyncConfig.parseEnv(),
  });

export const HolefeederConfig = {
  parseEnv: parseEnv,
};
