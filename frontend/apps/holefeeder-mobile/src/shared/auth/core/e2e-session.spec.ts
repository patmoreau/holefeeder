import { E2eSession, E2eSessionErrors } from '@/shared/auth/core/e2e-session';

const base64Url = (value: object): string =>
  Buffer.from(JSON.stringify(value)).toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');

const inOneHour = () => Math.floor(Date.now() / 1000) + 3600;

const aToken = (payload: object): string => `header.${base64Url(payload)}.signature`;

describe('E2eSession', () => {
  describe('parseLink', () => {
    it('reads the token and the subject claim', () => {
      const token = aToken({ sub: 'auth0|abc123', exp: inOneHour() });

      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${token}`);

      expect(result).toBeSuccessWithValue({
        token: token,
        expiresAt: expect.any(Number),
        user: { sub: 'auth0|abc123' },
      });
    });

    it('reads the expiry from the token', () => {
      const exp = inOneHour();

      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${aToken({ sub: 'auth0|abc123', exp: exp })}`);

      expect(result.isSuccess && result.value.expiresAt).toBe(exp);
    });

    it('enriches the user from optional query parameters', () => {
      const token = aToken({ sub: 'auth0|abc123', exp: inOneHour() });

      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${token}&email=someone%40example.com&name=Test%20User`);

      expect(result.isSuccess && result.value.user).toEqual({
        sub: 'auth0|abc123',
        email: 'someone@example.com',
        name: 'Test User',
      });
    });

    it('fails when the link is not the e2e-auth link', () => {
      const result = E2eSession.parseLink(`holefeeder://purchase?token=${aToken({ sub: 'auth0|abc123', exp: inOneHour() })}`);

      expect(result).toBeFailureWithErrors([E2eSessionErrors.invalidLink]);
    });

    it('fails when the token is missing', () => {
      const result = E2eSession.parseLink('holefeeder://e2e-auth');

      expect(result).toBeFailureWithErrors([E2eSessionErrors.missingToken]);
    });

    it('fails when the token is not a three-segment jwt', () => {
      const result = E2eSession.parseLink('holefeeder://e2e-auth?token=not-a-jwt');

      expect(result).toBeFailureWithErrors([E2eSessionErrors.invalidToken]);
    });

    it('fails when the token payload is not json', () => {
      const result = E2eSession.parseLink('holefeeder://e2e-auth?token=header.bm90LWpzb24.signature');

      expect(result).toBeFailureWithErrors([E2eSessionErrors.invalidToken]);
    });

    it('fails when the payload has no subject', () => {
      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${aToken({ exp: inOneHour() })}`);

      expect(result).toBeFailureWithErrors([E2eSessionErrors.missingSubject]);
    });

    it('fails when the payload has no expiry', () => {
      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${aToken({ sub: 'auth0|abc123' })}`);

      expect(result).toBeFailureWithErrors([E2eSessionErrors.missingExpiry]);
    });

    it('fails when the token has already expired', () => {
      const expired = Math.floor(Date.now() / 1000) - 1;

      const result = E2eSession.parseLink(`holefeeder://e2e-auth?token=${aToken({ sub: 'auth0|abc123', exp: expired })}`);

      expect(result).toBeFailureWithErrors([E2eSessionErrors.expiredToken]);
    });
  });
});
