import { aString, aUrl } from '@/shared/__tests__/string-for-test';
import { FetchRequest } from '@/shared/api/__tests__/fetch-for-test';

const defaultFetchRequest = (): FetchRequest => ({
  url: aUrl(),
  headers: { Authorization: `Bearer ${aString()}` },
  matchHeaders: true,
  matchUrl: (url: string, testUrl: string) => url === testUrl,
});

export const aFetchRequest = (overrides?: Partial<FetchRequest>) => ({ ...defaultFetchRequest(), ...overrides });
