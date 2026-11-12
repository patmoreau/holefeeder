import { type AuthenticationState, buildUrl, Result } from '@holefeeder/shared/core';

export type ApiConfig = {
  url: string;
  timeout: number;
};

export type Page<T> = { items: T[]; total: number };

export type ApiClient = {
  get: <T>(endpoint: string) => Promise<Result<T>>;
  getPage: <T>(endpoint: string) => Promise<Result<Page<T>>>;
  post: <TInput, TOutput>(endpoint: string, data?: TInput) => Promise<Result<TOutput>>;
  delete: (endpoint: string) => Promise<Result<void>>;
};

const problemCodeOf = async (response: Response): Promise<string[]> => {
  if (!(response.headers.get('content-type') ?? '').includes('json')) return [];
  const problem = (await response.json().catch(() => undefined)) as { code?: unknown } | undefined;
  return typeof problem?.code === 'string' ? [problem.code] : [];
};

export const ApiClient = (auth: AuthenticationState, config: ApiConfig): ApiClient => {
  const callApi = async <T>(
    endpoint: string,
    options: RequestInit = {},
    read?: (response: Response, data: unknown) => T
  ): Promise<Result<T>> => {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), config.timeout);

    try {
      const tokenInfo = await auth.getToken();
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
        ...(tokenInfo ? { Authorization: `Bearer ${tokenInfo.token}` } : {}),
      };

      const response = await fetch(buildUrl(new URL(config.url, window.location.origin).href, endpoint), {
        ...options,
        headers,
        signal: controller.signal,
      });

      if (!response.ok) {
        return Result.failure([...(await problemCodeOf(response)), response.statusText]);
      }

      const contentLength = response.headers.get('content-length');
      const contentType = response.headers.get('content-type') ?? '';
      const hasBody = contentLength === null ? true : parseInt(contentLength, 10) > 0;
      const hasJsonBody = hasBody && contentType.includes('application/json');
      const data: unknown = hasJsonBody ? await response.json() : undefined;
      return Result.success(read ? read(response, data) : (data as T));
    } catch (error) {
      return Result.failure([(error as Error).message]);
    } finally {
      clearTimeout(timeoutId);
    }
  };

  return {
    get: <T>(endpoint: string) => callApi<T>(endpoint, { method: 'GET' }),
    getPage: <T>(endpoint: string) =>
      callApi<Page<T>>(endpoint, { method: 'GET' }, (response, data) => ({
        items: (data as T[] | undefined) ?? [],
        total: Number(response.headers.get('x-total-count') ?? 0),
      })),
    post: <TInput, TOutput>(endpoint: string, data?: TInput) =>
      callApi<TOutput>(endpoint, { method: 'POST', body: data ? JSON.stringify(data) : undefined }),
    delete: (endpoint: string) => callApi<void>(endpoint, { method: 'DELETE' }),
  };
};
