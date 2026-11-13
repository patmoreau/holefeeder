import { aUrl } from '@holefeeder/shared/testkit';
import { ApiConfig } from '@/shared/api/api-config';

const defaultApiConfig = (): ApiConfig => ({
  url: aUrl(),
  timeout: 1000,
  logRequests: true,
  simulateNetworkDelay: 0,
  cacheRequests: true,
});

export const anApiConfig = (overrides?: Partial<ApiConfig>) => ({ ...defaultApiConfig(), ...overrides });
