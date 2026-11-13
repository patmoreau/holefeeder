import { describe, expect, it } from 'vitest';
import { AccessToken } from './access-token';

const segment = (value: object) => btoa(JSON.stringify(value)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');

const tokenWith = (payload: object) => `${segment({ alg: 'RS256' })}.${segment(payload)}.signature`;

describe('AccessToken', () => {
  it('ShouldReadTheExpiryFromTheToken', () => {
    const token = tokenWith({ exp: 1893456000 });
    expect(AccessToken.info(token)).toEqual({ token: token, expiresAt: 1893456000 });
  });

  it('ShouldReadAPayloadThatUsesTheBase64UrlAlphabet', () => {
    const token = tokenWith({ exp: 1893456000, name: '??>>' });
    expect(token.split('.')[1]).toMatch(/[-_]/);
    expect(AccessToken.info(token)).toEqual({ token: token, expiresAt: 1893456000 });
  });

  it('ShouldRejectATokenWithoutAnExpiry', () => {
    expect(AccessToken.info(tokenWith({ sub: 'auth0|123' }))).toBeUndefined();
  });

  it('ShouldRejectAMalformedToken', () => {
    expect(AccessToken.info('not-a-token')).toBeUndefined();
  });
});
