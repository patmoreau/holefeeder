import { AuthConfig } from '@/shared/auth/core/auth-config';
import { authorizeParameters } from '@/shared/auth/core/authorize-parameters';

describe('authorizeParameters', () => {
  const config: AuthConfig = {
    domain: 'holefeeder.example.auth0.com',
    clientId: 'a-client-id',
    audience: 'https://holefeeder-api.example.app',
    redirectUri: 'holefeeder://callback',
    logoutRedirectUri: 'holefeeder://logout',
    scope: 'openid profile email read:user write:user',
  };

  it('carries the configured scope, audience and redirect', () => {
    const parameters = authorizeParameters(config);

    expect(parameters).toEqual({
      scope: config.scope,
      audience: config.audience,
      redirectUrl: config.redirectUri,
    });
  });

  it('opens the login page when signing in', () => {
    const parameters = authorizeParameters(config, { signUp: false });

    expect(parameters.additionalParameters).toBeUndefined();
  });

  it('opens the signup page when creating an account', () => {
    const parameters = authorizeParameters(config, { signUp: true });

    expect(parameters.additionalParameters).toEqual({ screen_hint: 'signup' });
  });

  it('keeps the rest of the parameters when creating an account', () => {
    const parameters = authorizeParameters(config, { signUp: true });

    expect(parameters).toMatchObject({
      scope: config.scope,
      audience: config.audience,
      redirectUrl: config.redirectUri,
    });
  });
});
