import { AuthenticationState, buildUrl, Logger, Result } from '@holefeeder/shared/core';
import { ApiConfig } from '@/shared/api/api-config';
import { apiErrorForStatus } from '@/shared/api/api-errors';

const logger = Logger.create('api-client');

export type ApiClient = {
  get: <TOutput>(endpoint: string, req?: RequestInit) => Promise<Result<TOutput>>;
  post: <TInput, TOutput>(endpoint: string, data?: TInput, req?: RequestInit) => Promise<Result<TOutput>>;
};

export const ApiClient = (authenticationState: AuthenticationState, apiConfig: ApiConfig): ApiClient => {
  const executeFetch = async (endpoint: string, init: RequestInit): Promise<Response> => {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), apiConfig.timeout);

    try {
      return await fetch(buildUrl(apiConfig.url, endpoint), {
        ...init,
        signal: controller.signal,
      });
    } finally {
      clearTimeout(timeoutId);
    }
  };

  const callApi = async <T>(endpoint: string, options: RequestInit = {}): Promise<Result<T>> => {
    try {
      const token = await authenticationState.getToken();
      const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { Authorization: `Bearer ${token.token}` } : {}),
      };

      const obfuscateHeaders = (h: Record<string, string>) =>
        Object.fromEntries(Object.entries(h).map(([k, v]) => [k, k.toLowerCase() === 'authorization' ? `${v.slice(0, 10)}…[redacted]` : v]));

      if (apiConfig.logRequests) {
        logger.info('Request:', {
          url: apiConfig.url,
          endpoint,
          method: options.method,
          headers: obfuscateHeaders(headers as Record<string, string>),
          body: options.body,
        });
      }

      const doFetch = () =>
        executeFetch(endpoint, {
          ...options,
          headers,
        });

      const response =
        apiConfig.simulateNetworkDelay > 0
          ? await new Promise<Response>((resolve) => setTimeout(() => resolve(doFetch()), apiConfig.simulateNetworkDelay))
          : await doFetch();

      if (apiConfig.logRequests) {
        if (response.ok) {
          logger.info('Response:', { status: response.status, headers: obfuscateHeaders(Object.fromEntries(response.headers)) });
        } else {
          logger.error('Error Response:', {
            status: response.status,
            headers: obfuscateHeaders(Object.fromEntries(response.headers)),
            statusText: response.statusText,
          });
        }
      }

      if (!response.ok) {
        return Result.failure([apiErrorForStatus(response.status)]);
      }

      const contentLength = response.headers.get('content-length');
      const contentType = response.headers.get('content-type') ?? '';
      const hasBody = contentLength === null ? true : parseInt(contentLength, 10) > 0;
      const hasJsonBody = hasBody && contentType.includes('application/json');
      const data = hasJsonBody ? await response.json() : undefined;
      return Result.success(data as T);
    } catch (error) {
      const apiError = error as Error;
      return Result.failure([apiError.message]);
    }
  };

  const get = async <TOutput>(endpoint: string, req?: RequestInit): Promise<Result<TOutput>> =>
    callApi<TOutput>(endpoint, { ...req, method: 'GET' });

  const post = async <TInput, TOutput>(endpoint: string, data?: TInput, req?: RequestInit): Promise<Result<TOutput>> => {
    const body = data ? JSON.stringify(data) : undefined;
    return callApi<TOutput>(endpoint, { ...req, method: 'POST', body });
  };

  return {
    get: get,
    post: post,
  };
};
