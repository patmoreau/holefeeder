import { buildUrl } from '@holefeeder/shared/core';
import { anApiConfig } from '@/shared/api/__tests__/api-config-for-test';
import { FetchForTest } from '@/shared/api/__tests__/fetch-for-test';
import { aFetchRequest } from '@/shared/api/__tests__/fetch-request-for-test';
import { aFetchResponse } from '@/shared/api/__tests__/fetch-response-for-test';
import { ApiErrors } from '@/shared/api/api-errors';
import { usersApi } from '@/shared/api/users-api';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { aTokenInfo } from '@/shared/auth/__tests__/token-info-for-test';

describe('users-api', () => {
  const apiConfig = anApiConfig();
  const tokenInfo = aTokenInfo();
  const authenticationState = anAuthenticationState({ getToken: () => Promise.resolve(tokenInfo) });

  const requestFor = (endpoint: string) =>
    aFetchRequest({
      url: buildUrl(apiConfig.url, endpoint),
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${tokenInfo.token}`,
      },
    });

  const jsonResponse = (body: unknown) => {
    const serialized = JSON.stringify(body);
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');
    headers.append('Content-Length', serialized.length.toString());
    return aFetchResponse({ body: serialized, status: 200, ok: true, headers: headers });
  };

  let fetchForTest: FetchForTest;
  const api = usersApi(authenticationState, apiConfig);

  beforeEach(() => {
    fetchForTest = FetchForTest();
  });

  afterEach(() => {
    fetchForTest.restore();
  });

  describe('getMe', () => {
    it('should return the current user', async () => {
      const currentUser = { id: '6f2a1e5c-9b3d-4a7e-8c10-2d5f7b9a4c31' };
      fetchForTest.simulate({ request: requestFor('/api/v2/users/me'), response: jsonResponse(currentUser) });

      const result = await api.getMe();

      expect(result).toBeSuccessWithValue(currentUser);
    });

    it('should report an unregistered caller as not found', async () => {
      fetchForTest.simulate({
        request: requestFor('/api/v2/users/me'),
        response: aFetchResponse({ status: 404, ok: false, statusText: 'Not Found', body: {} }),
      });

      const result = await api.getMe();

      expect(result).toBeFailureWithErrors([ApiErrors.notFound]);
    });
  });

  describe('register', () => {
    it('should return the newly registered user', async () => {
      const currentUser = { id: 'b81d3f6a-5c2e-4f19-9a7b-3e6d8c1f0a24' };
      fetchForTest.simulate({ request: requestFor('/api/v2/users/register'), response: jsonResponse(currentUser) });

      const result = await api.register();

      expect(result).toBeSuccessWithValue(currentUser);
    });

    it('should report an already registered caller as a bad request', async () => {
      fetchForTest.simulate({
        request: requestFor('/api/v2/users/register'),
        response: aFetchResponse({ status: 400, ok: false, statusText: 'Bad Request', body: {} }),
      });

      const result = await api.register();

      expect(result).toBeFailureWithErrors([ApiErrors.badRequest]);
    });
  });
});
