import { LoginOptions } from '@holefeeder/shared/core';
import { AuthConfig } from '@/shared/auth/core/auth-config';

export type AuthorizeParameters = {
  scope: string;
  audience: string;
  redirectUrl: string;
  additionalParameters?: Record<string, string>;
};

// Auth0 opens its login page by default and its signup page when asked, through
// screen_hint. Requires New Universal Login on the tenant; on Classic the hint is
// ignored and the user lands on login, which is a degraded path rather than a
// broken one.
export const authorizeParameters = (config: AuthConfig, options?: LoginOptions): AuthorizeParameters => ({
  scope: config.scope,
  audience: config.audience,
  redirectUrl: config.redirectUri,
  ...(options?.signUp ? { additionalParameters: { screen_hint: 'signup' } } : {}),
});
