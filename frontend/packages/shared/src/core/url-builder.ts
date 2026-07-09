/**
 * Joins a base URL and an endpoint path, correctly handling trailing slashes on
 * the base and leading slashes on the endpoint so that path segments in the
 * base (e.g. `/gateway`) are always preserved.
 *
 * @example
 * buildUrl('https://example.com/gateway', '/api/v2/resource')
 * // → 'https://example.com/gateway/api/v2/resource'
 *
 * buildUrl('https://example.com/gateway/', 'api/v2/resource')
 * // → 'https://example.com/gateway/api/v2/resource'
 */
export const buildUrl = (base: string, endpoint: string): string => {
  const normalizedBase = base.endsWith('/') ? base : `${base}/`;
  const normalizedEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
  return new URL(normalizedEndpoint, normalizedBase).toString();
};
