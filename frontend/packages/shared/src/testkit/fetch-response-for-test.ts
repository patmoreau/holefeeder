import { FetchResponse } from './fetch-for-test';

const defaultFetchResponse = (): FetchResponse => ({
  body: Math.random().toString(36).slice(2, 10),
  status: 200,
  ok: true,
  statusText: 'OK',
  headers: new Headers(),
});

export const aFetchResponse = (overrides?: Partial<FetchResponse>) => ({ ...defaultFetchResponse(), ...overrides });
