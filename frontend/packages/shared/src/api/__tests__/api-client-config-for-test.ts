import { aUrl } from '../../core/__tests__/helpers/string-for-test';
import { ApiClientConfig } from '../api-client';

const defaultApiClientConfig = (): ApiClientConfig => ({
  url: aUrl(),
  timeout: 1000,
  logRequests: true,
  simulateNetworkDelay: 0,
});

export const anApiClientConfig = (overrides?: Partial<ApiClientConfig>): ApiClientConfig => ({
  ...defaultApiClientConfig(),
  ...overrides,
});
