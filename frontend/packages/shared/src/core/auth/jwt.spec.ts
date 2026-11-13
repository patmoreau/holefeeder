import { Jwt, JwtErrors } from './jwt';

const segment = (value: object) => btoa(JSON.stringify(value)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');

const tokenWith = (payload: object) => `${segment({ alg: 'RS256' })}.${segment(payload)}.signature`;

describe('Jwt', () => {
  describe('payload', () => {
    it('decodes the claims of a token', () => {
      expect(Jwt.payload(tokenWith({ sub: 'auth0|123', exp: 1893456000 }))).toBeSuccessWithValue({ sub: 'auth0|123', exp: 1893456000 });
    });

    it('decodes a payload that uses the base64url alphabet', () => {
      const payload = { name: '??>>' };
      expect(segment(payload)).toMatch(/[-_]/);
      expect(Jwt.payload(tokenWith(payload))).toBeSuccessWithValue(payload);
    });

    it('fails for a token without three segments', () => {
      expect(Jwt.payload('not-a-token')).toBeFailureWithErrors([JwtErrors.invalid]);
    });

    it('fails for a payload that is not JSON', () => {
      expect(Jwt.payload(`header.${btoa('nope')}.signature`)).toBeFailureWithErrors([JwtErrors.invalid]);
    });

    it('fails for a payload that is not an object', () => {
      expect(Jwt.payload(`header.${btoa('42')}.signature`)).toBeFailureWithErrors([JwtErrors.invalid]);
    });
  });
});
