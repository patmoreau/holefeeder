import { buildUrl } from '../core';
import { FetchForTest } from '../testkit/fetch-for-test';
import { aFetchRequest } from '../testkit/fetch-request-for-test';
import { aFetchResponse } from '../testkit/fetch-response-for-test';
import { anApiClientConfig } from './__tests__/api-client-config-for-test';
import { aTokenInfo, anAuthenticationState } from './__tests__/authentication-state-for-test';
import { ApiClient } from './api-client';
import { ApiErrors } from './api-errors';

describe('ApiClient', () => {
  const apiConfig = anApiClientConfig();
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

    it('should report the problem code ahead of the status error', async () => {
      const authState = anAuthenticationState();
      const apiClient = ApiClient(authState, apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({
          status: 409,
          ok: false,
          statusText: 'Conflict',
          headers: new Headers({ 'Content-Type': 'application/problem+json' }),
          body: { code: 'category-name-taken' },
        }),
      });

      const result = await apiClient.post(endpoint);

      expect(result).toBeFailureWithErrors(['category-name-taken', ApiErrors.conflict]);
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

  describe('getPage', () => {
    it('should return the items with the total count from the response header', async () => {
      const apiClient = ApiClient(anAuthenticationState(), apiConfig);
      const items = [{ id: 'first' }, { id: 'second' }];

      fetchForTest.simulate({
        request: aFetchRequest({ url, method: 'GET', matchHeaders: false }),
        response: aFetchResponse({
          headers: new Headers({ 'Content-Type': 'application/json', 'X-Total-Count': '12' }),
          body: items,
        }),
      });

      const result = await apiClient.getPage(endpoint);

      expect(result).toBeSuccessWithValue({ items: items, total: 12 });
    });

    it('should return an empty page when the response has no body', async () => {
      const apiClient = ApiClient(anAuthenticationState(), apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 204, statusText: 'No Content', headers: new Headers({ 'Content-Length': '0' }) }),
      });

      const result = await apiClient.getPage(endpoint);

      expect(result).toBeSuccessWithValue({ items: [], total: 0 });
    });

    it('should report a failed page request', async () => {
      const apiClient = ApiClient(anAuthenticationState(), apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 500, ok: false, statusText: 'Internal Server Error', body: {} }),
      });

      const result = await apiClient.getPage(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.serverError]);
    });
  });

  describe('delete', () => {
    it('should send a DELETE request with the bearer token', async () => {
      const tokenInfo = aTokenInfo();
      const apiClient = ApiClient(anAuthenticationState({ getToken: () => Promise.resolve(tokenInfo) }), apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({
          url,
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${tokenInfo.token}` },
        }),
        response: aFetchResponse({ status: 204, statusText: 'No Content', headers: new Headers({ 'Content-Length': '0' }) }),
      });

      const result = await apiClient.delete(endpoint);

      expect(result).toBeSuccessWithValue(undefined);
    });

    it('should report a missing resource as not found', async () => {
      const apiClient = ApiClient(anAuthenticationState(), apiConfig);

      fetchForTest.simulate({
        request: aFetchRequest({ url, matchHeaders: false }),
        response: aFetchResponse({ status: 404, ok: false, statusText: 'Not Found', body: {} }),
      });

      const result = await apiClient.delete(endpoint);

      expect(result).toBeFailureWithErrors([ApiErrors.notFound]);
    });
  });
});
