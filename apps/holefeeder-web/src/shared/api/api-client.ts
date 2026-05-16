import { type AuthenticationState, buildUrl, Result } from '@holefeeder/core';

export type ApiConfig = {
  url: string;
  timeout: number;
};

export type ApiClient = {
  get: <T>(endpoint: string) => Promise<Result<T>>;
  post: <TInput, TOutput>(endpoint: string, data?: TInput) => Promise<Result<TOutput>>;
};

export const ApiClient = (auth: AuthenticationState, config: ApiConfig): ApiClient => {
  const callApi = async <T>(endpoint: string, options: RequestInit = {}): Promise<Result<T>> => {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), config.timeout);

    try {
      const tokenInfo = await auth.getToken();
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(tokenInfo ? { Authorization: `Bearer ${tokenInfo.token}` } : {}),
      };

      const response = await fetch(buildUrl(config.url, endpoint), {
        ...options,
        headers,
        signal: controller.signal,
      });

      if (!response.ok) {
        return Result.failure([response.statusText]);
      }

      const contentLength = response.headers.get('content-length');
      const contentType = response.headers.get('content-type') ?? '';
      const hasBody = contentLength === null ? true : parseInt(contentLength, 10) > 0;
      const hasJsonBody = hasBody && contentType.includes('application/json');
      const data = hasJsonBody ? ((await response.json()) as T) : undefined;
      return Result.success(data as T);
    } catch (error) {
      return Result.failure([(error as Error).message]);
    } finally {
      clearTimeout(timeoutId);
    }
  };

  return {
    get: <T>(endpoint: string) => callApi<T>(endpoint, { method: 'GET' }),
    post: <TInput, TOutput>(endpoint: string, data?: TInput) =>
      callApi<TOutput>(endpoint, { method: 'POST', body: data ? JSON.stringify(data) : undefined }),
  };
};
