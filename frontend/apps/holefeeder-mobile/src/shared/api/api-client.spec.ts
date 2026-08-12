import { buildUrl } from '@holefeeder/shared/core';
import { anApiConfig } from '@/shared/api/__tests__/api-config-for-test';
import { FetchForTest } from '@/shared/api/__tests__/fetch-for-test';
import { aFetchRequest } from '@/shared/api/__tests__/fetch-request-for-test';
import { aFetchResponse } from '@/shared/api/__tests__/fetch-response-for-test';
import { ApiClient } from '@/shared/api/api-client';
import { ApiErrors } from '@/shared/api/api-errors';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { aTokenInfo } from '@/shared/auth/__tests__/token-info-for-test';

describe('ApiClient', () => {
  const apiConfig = anApiConfig();
  const endpoint = '/test-endpoint';
  const url = buildUrl(apiConfig.url, endpoint);

  let fetchForTest: FetchForTest;

  beforeEach(() => {
    fetchForTest = FetchForTest();
  });

  afterEach(() => {
    fetchForTest.restore();
  });

  describe('post', () => {
    it('should send a POST request with the correct headers and body', async () => {
      const tokenInfo = aTokenInfo();
      const authState = anAuthenticationState({ getToken: () => Promise.resolve(tokenInfo) });
      const apiClient = ApiClient(authState, apiConfig);

      const requestData = { foo: 'bar' };
      const responseData = { success: true };
      const responseBody = JSON.stringify(responseData);
      const expectedHeaders = new Headers();
      expectedHeaders.append('Content-Type', 'application/json');
      expectedHeaders.append('Content-Length', responseBody.length.toString());

      fetchForTest.simulate({
        request: aFetchRequest({
          url,
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${tokenInfo.token}`,
          },
        }),
        response: aFetchResponse({
          body: responseBody,
          status: 200,
          ok: true,
          headers: expectedHeaders,
        }),
      });

      const result = await apiClient.post<typeof requestData, typeof responseData>(endpoint, requestData);

      expect(result).toBeSuccessWithValue(responseData);
    });

    it('should handle request without token', async () => {
      const authState = anAuthenticationState({ getToken: () => Promise.resolve(undefined) });
      const apiClient = ApiClient(authState, apiConfig);

      const responseData = { success: true };
      const responseBody = JSON.stringify(responseData);
      const expectedHeaders = new Headers();
      expectedHeaders.append('Content-Type', 'application/json');
      expectedHeaders.append('Content-Length', responseBody.length.toString());

      fetchForTest.simulate({
        request: aFetchRequest({
          url,
          headers: {
            'Content-Type': 'application/json',
          },
        }),
        response: aFetchResponse({
          body: responseBody,
          status: 200,
          ok: true,
          headers: expectedHeaders,
        }),
      });

      const result = await apiClient.post(endpoint);

      expect(result).toBeSuccessWithValue(responseData);
    });

    it('should return failure when response is not ok', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({
          status: 400,
          ok: false,
          statusText: 'Bad Request',
          body: { error: 'some error' },
        }),
      });

      const result = await apiClient.post(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.badRequest]);
    });

    it('should report an unauthorized response distinctly', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 401, ok: false, statusText: 'Unauthorized', body: {} }),
      });

      const result = await apiClient.post(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.unauthorized]);
    });

    it('should return failure when fetch throws an error', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: new Error('Network failure'),
      });

      const result = await apiClient.post(endpoint);

      expect(result.isFailure).toBe(true);
      if (result.isFailure) {
        expect(result.errors).toEqual(['Network failure']);
      }
    });
  });

  describe('get', () => {
    it('should send a GET request with the correct headers and return the body', async () => {
      const tokenInfo = aTokenInfo();
      const authState = anAuthenticationState({ getToken: () => Promise.resolve(tokenInfo) });
      const apiClient = ApiClient(authState, apiConfig);

      const responseData = { id: 'a2e7f0f0-1a1b-4c3d-9e8f-0a1b2c3d4e5f' };
      const responseBody = JSON.stringify(responseData);
      const expectedHeaders = new Headers();
      expectedHeaders.append('Content-Type', 'application/json');
      expectedHeaders.append('Content-Length', responseBody.length.toString());

      fetchForTest.simulate({
        request: aFetchRequest({
          url,
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${tokenInfo.token}`,
          },
        }),
        response: aFetchResponse({
          body: responseBody,
          status: 200,
          ok: true,
          headers: expectedHeaders,
        }),
      });

      const result = await apiClient.get<typeof responseData>(endpoint);

      expect(result).toBeSuccessWithValue(responseData);
    });

    it('should succeed with no value when the response has no body', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      const expectedHeaders = new Headers();
      expectedHeaders.append('Content-Length', '0');

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 204, ok: true, statusText: 'No Content', headers: expectedHeaders }),
      });

      const result = await apiClient.get(endpoint);

      expect(result).toBeSuccessWithValue(undefined);
    });

    it('should report a missing resource as not found', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 404, ok: false, statusText: 'Not Found', body: {} }),
      });

      const result = await apiClient.get(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.notFound]);
    });

    it('should report a server failure distinctly from a missing resource', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 500, ok: false, statusText: 'Internal Server Error', body: {} }),
      });

      const result = await apiClient.get(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.serverError]);
    });
  });
});
