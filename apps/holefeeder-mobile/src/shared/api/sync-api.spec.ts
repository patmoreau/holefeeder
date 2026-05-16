import { anApiConfig } from '@/shared/api/__tests__/api-config-for-test';
import { FetchForTest } from '@/shared/api/__tests__/fetch-for-test';
import { aFetchRequest } from '@/shared/api/__tests__/fetch-request-for-test';
import { aFetchResponse } from '@/shared/api/__tests__/fetch-response-for-test';
import { syncApi } from '@/shared/api/sync-api';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { aTokenInfo } from '@/shared/auth/__tests__/token-info-for-test';
import { buildUrl } from '@/shared/core/url-builder';

describe('sync-api', () => {
  const apiConfig = anApiConfig();
  const endpoint = '/api/v2/sync/powersync';
  const url = buildUrl(apiConfig.url, endpoint);
  const tokenInfo = aTokenInfo();
  const authenticationState = anAuthenticationState({ getToken: () => Promise.resolve(tokenInfo) });
  const request = aFetchRequest({
    url,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${tokenInfo.token}`,
    },
  });

  let fetchForTest: FetchForTest;
  const sync = syncApi(authenticationState, apiConfig);

  beforeEach(() => {
    fetchForTest = FetchForTest();
  });

  afterEach(() => {
    fetchForTest.restore();
  });

  describe('upload', () => {
    it('should upload data with transaction ID', async () => {
      fetchForTest.simulate({ request: request, response: aFetchResponse({ status: 204, ok: true, body: undefined }) });
      const transactionId = 123;
      const operations: any[] = [{ op: 'PUT', table: 'test', data: {} }];

      const result = await sync.upload({ transactionId, operations });

      expect(result).toBeSuccessWithValue(undefined);
    });

    it('should upload data without transaction ID', async () => {
      fetchForTest.simulate({ request: request, response: aFetchResponse({ status: 204, ok: true, body: undefined }) });
      const transactionId = undefined;
      const operations: any[] = [{ op: 'PUT', table: 'test', data: {} }];

      const result = await sync.upload({ transactionId, operations });

      expect(result).toBeSuccessWithValue(undefined);
    });

    it('handles bad requests', async () => {
      fetchForTest.simulate({ request: request, response: aFetchResponse({ status: 400, ok: false, statusText: 'Bad request' }) });
      const transactionId = undefined;
      const operations: any[] = [{ op: 'PUT', table: 'test', data: {} }];

      const result = await sync.upload({ transactionId, operations });

      expect(result).toBeFailureWithErrors(['Bad request']);
    });

    it('should handle API errors', async () => {
      fetchForTest.simulate({ request: request, response: new Error('HTTP 500: Internal Server Error') });
      const operations: any[] = [];

      const result = await sync.upload({ operations });

      expect(result).toBeFailureWithErrors(['HTTP 500: Internal Server Error']);
    });
  });
});
