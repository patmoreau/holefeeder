import { aString } from '@/shared/__tests__/string-for-test';
import { FetchResponse } from '@/shared/api/__tests__/fetch-for-test';

const defaultFetchResponse = (): FetchResponse => ({
  body: aString(),
  status: 200,
  ok: true,
  statusText: 'OK',
  headers: new Headers(),
});

export const aFetchResponse = (overrides?: Partial<FetchResponse>) => ({ ...defaultFetchResponse(), ...overrides });
