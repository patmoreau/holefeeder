export type FetchRequest = {
  url: string;
  headers?: Record<string, string>;
  matchHeaders: boolean;
  matchUrl: (url: string, testUrl: string) => boolean;
};
export type FetchResponse = { status: number; body: unknown; ok: boolean; statusText: string; headers: Headers };
export type FetchPair = { request: FetchRequest; response: FetchResponse | Error };
export type FetchForTest = {
  restore: () => void;
  simulate: (...pairs: FetchPair[]) => void;
};

export const FetchForTest = (): FetchForTest => {
  const originalFetch = globalThis.fetch;

  const restore = () => {
    globalThis.fetch = originalFetch;
  };

  const simulate = (...pairs: FetchPair[]) => {
    globalThis.fetch = fetch(pairs);
  };

  const fetch = (requests: FetchPair[]) => async (url: string | URL | Request, options?: RequestInit) => {
    const urlString = url.toString();
    const matched = requests.find((req) => req.request.matchUrl(urlString, req.request.url));

    expect(matched).toBeDefined();

    const matchedPair = matched!;

    if (matchedPair.request.matchHeaders) {
      expect(options?.headers).toEqual(matchedPair.request.headers);
    }

    if (matchedPair.response instanceof Error) {
      throw matchedPair.response;
    }

    const body = matchedPair.response.body;
    return {
      status: matchedPair.response.status,
      ok: matchedPair.response.ok,
      statusText: matchedPair.response.statusText,
      headers: matchedPair.response.headers,
      // Real Response.json() parses the body and hands back an object. Returning the
      // raw string instead let a double parse in the client look correct in tests.
      json: async () => JSON.parse(typeof body === 'string' ? body : JSON.stringify(body)),
    } as Response;
  };

  return {
    restore: restore,
    simulate: simulate,
  };
};
