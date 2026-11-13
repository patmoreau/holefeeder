import type { ApiClientConfig } from '@holefeeder/shared/api';

export const apiConfig: ApiClientConfig = {
  url: new URL(import.meta.env.VITE_API_BASE_URL as string, window.location.origin).href,
  timeout: 10_000,
  logRequests: false,
  simulateNetworkDelay: 0,
};
