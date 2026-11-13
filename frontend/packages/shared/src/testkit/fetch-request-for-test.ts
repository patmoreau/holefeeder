import { FetchRequest } from './fetch-for-test';

const aSuffix = () => Math.random().toString(36).slice(2, 10);

const defaultFetchRequest = (): FetchRequest => ({
  url: `https://${aSuffix()}.example.com`,
  headers: { Authorization: `Bearer ${aSuffix()}` },
  matchHeaders: true,
  matchUrl: (url: string, testUrl: string) => url === testUrl,
});

export const aFetchRequest = (overrides?: Partial<FetchRequest>) => ({ ...defaultFetchRequest(), ...overrides });
