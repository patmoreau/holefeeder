import { buildUrl } from './url-builder';

describe('buildUrl', () => {
  it('appends endpoint to base with path segment', () => {
    expect(buildUrl('https://example.com/gateway', '/api/v2/resource')).toBe('https://example.com/gateway/api/v2/resource');
  });

  it('handles trailing slash on base', () => {
    expect(buildUrl('https://example.com/gateway/', '/api/v2/resource')).toBe('https://example.com/gateway/api/v2/resource');
  });

  it('handles endpoint without leading slash', () => {
    expect(buildUrl('https://example.com/gateway', 'api/v2/resource')).toBe('https://example.com/gateway/api/v2/resource');
  });

  it('handles both trailing slash on base and no leading slash on endpoint', () => {
    expect(buildUrl('https://example.com/gateway/', 'api/v2/resource')).toBe('https://example.com/gateway/api/v2/resource');
  });

  it('handles base without path segment', () => {
    expect(buildUrl('https://example.com', '/api/v2/resource')).toBe('https://example.com/api/v2/resource');
  });
});
