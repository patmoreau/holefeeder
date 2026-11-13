import { FetchRequest } from './fetch-for-test';
import { aString, aUrl } from './string-for-test';

const defaultFetchRequest = (): FetchRequest => ({
  url: aUrl(),
  headers: { Authorization: `Bearer ${aString()}` },
  matchHeaders: true,
  matchUrl: (url: string, testUrl: string) => url === testUrl,
});

export const aFetchRequest = (overrides?: Partial<FetchRequest>) => ({ ...defaultFetchRequest(), ...overrides });
