import { EnvVariable } from '@/shared/core/env/env-variable';
import { Result } from '@/shared/core/result';

export type ApiConfig = {
  url: string;
  timeout: number;
  logRequests: boolean;
  simulateNetworkDelay: number;
  cacheRequests: boolean;
};

export const ApiConfigEnvVariableNames = {
  apiBaseUrl: 'EXPO_PUBLIC_API_BASE_URL',
  apiTimeout: 'EXPO_PUBLIC_API_TIMEOUT',
  apiLogRequests: 'EXPO_PUBLIC_API_LOG_REQUEST',
  simulateNetworkDelay: 'EXPO_PUBLIC_SIMULATE_NETWORK_DELAY',
  cacheRequests: 'EXPO_PUBLIC_CACHE_REQUESTS',
};

const parseEnv = (): Result<ApiConfig> => {
  const url = EnvVariable.read(ApiConfigEnvVariableNames.apiBaseUrl);
  const timeout = EnvVariable.read(ApiConfigEnvVariableNames.apiTimeout);
  const logRequests = EnvVariable.read(ApiConfigEnvVariableNames.apiLogRequests);
  const simulateNetworkDelay = EnvVariable.read(ApiConfigEnvVariableNames.simulateNetworkDelay);
  const cacheRequests = EnvVariable.read(ApiConfigEnvVariableNames.cacheRequests);

  if (url.isFailure) return url;
  if (timeout.isFailure) return timeout;
  if (logRequests.isFailure) return logRequests;
  if (simulateNetworkDelay.isFailure) return simulateNetworkDelay;
  if (cacheRequests.isFailure) return cacheRequests;

  return Result.success({
    url: url.value,
    timeout: timeout.value ? parseInt(timeout.value, 10) : 10000,
    logRequests: logRequests.value.toLowerCase() === 'true',
    simulateNetworkDelay: simulateNetworkDelay.value ? parseInt(simulateNetworkDelay.value, 10) : 0,
    cacheRequests: cacheRequests.value.toLowerCase() === 'true',
  });
};

export const ApiConfig = {
  parseEnv: parseEnv,
} as const;
