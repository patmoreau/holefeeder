import { FetchResponse } from './fetch-for-test';
import { aString } from './string-for-test';

const defaultFetchResponse = (): FetchResponse => ({
  body: aString(),
  status: 200,
  ok: true,
  statusText: 'OK',
  headers: new Headers(),
});

export const aFetchResponse = (overrides?: Partial<FetchResponse>) => ({ ...defaultFetchResponse(), ...overrides });
