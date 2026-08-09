import { Result, TokenInfo, User } from '@holefeeder/shared/core';

export const E2eSessionErrors = {
  invalidLink: 'invalid-link',
  missingToken: 'missing-token',
  invalidToken: 'invalid-token',
  missingSubject: 'missing-subject',
  missingExpiry: 'missing-expiry',
  expiredToken: 'expired-token',
};

export type E2eSessionData = TokenInfo & { user: User };

const E2E_TOKEN_PARAM = 'e2e-auth-token';

type JwtPayload = { sub?: unknown; exp?: unknown };

// Hand-rolled rather than using URL: React Native's URL has no searchParams, and
// Buffer does not exist there either, so both would pass in Jest and crash on device.
const parseQuery = (query: string): Record<string, string> => {
  const params: Record<string, string> = {};
  for (const pair of query.split('&')) {
    if (!pair) continue;
    const separator = pair.indexOf('=');
    if (separator < 0) continue;
    const key = decodeURIComponent(pair.slice(0, separator).replace(/\+/g, ' '));
    params[key] = decodeURIComponent(pair.slice(separator + 1).replace(/\+/g, ' '));
  }
  return params;
};

const decodePayload = (token: string): Result<JwtPayload> => {
  const segments = token.split('.');
  if (segments.length !== 3) return Result.failure([E2eSessionErrors.invalidToken]);

  try {
    const normalized = segments[1].replace(/-/g, '+').replace(/_/g, '/');
    const payload: unknown = JSON.parse(atob(normalized));
    if (typeof payload !== 'object' || payload === null) return Result.failure([E2eSessionErrors.invalidToken]);
    return Result.success(payload as JwtPayload);
  } catch {
    return Result.failure([E2eSessionErrors.invalidToken]);
  }
};

// Accepts holefeeder://?e2e-auth-token=<jwt>[&email=…][&name=…]. The link targets the
// root deliberately: expo-router turns any other host into a route, and an unknown
// route crashes the render. The expiry and the subject come from the token itself, so
// a link cannot claim a session the token does not actually grant.
const parseLink = (link: string): Result<E2eSessionData> => {
  const match = /^[a-z][a-z0-9+.-]*:\/\/([^/?#]*)(\/?)(?:\?([^#]*))?$/i.exec(link);
  if (!match || match[1] !== '') return Result.failure([E2eSessionErrors.invalidLink]);

  const params = parseQuery(match[3] ?? '');

  const token = params[E2E_TOKEN_PARAM];
  if (!token) return Result.failure([E2eSessionErrors.missingToken]);

  const payloadResult = decodePayload(token);
  if (payloadResult.isFailure) return Result.failure(payloadResult.errors);

  const { sub, exp } = payloadResult.value;
  if (typeof sub !== 'string' || sub === '') return Result.failure([E2eSessionErrors.missingSubject]);
  if (typeof exp !== 'number') return Result.failure([E2eSessionErrors.missingExpiry]);
  if (exp <= Math.floor(Date.now() / 1000)) return Result.failure([E2eSessionErrors.expiredToken]);

  return Result.success({
    token: token,
    // Seconds since the epoch, matching what react-native-auth0 reports.
    expiresAt: exp,
    user: {
      sub: sub,
      ...(params.email ? { email: params.email } : {}),
      ...(params.name ? { name: params.name } : {}),
    },
  });
};

export const E2eSession = {
  parseLink: parseLink,
};
