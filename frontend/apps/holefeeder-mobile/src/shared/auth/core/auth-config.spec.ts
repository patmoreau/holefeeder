import { AuthConfig } from '@/shared/auth/core/auth-config';

describe('AuthConfig', () => {
  const originalDomain = process.env.EXPO_PUBLIC_AUTH0_DOMAIN;
  const originalClientId = process.env.EXPO_PUBLIC_AUTH0_CLIENT_ID;
  const originalAudience = process.env.EXPO_PUBLIC_AUTH0_AUDIENCE;
  const originalRedirectUri = process.env.EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI;
  const originalLogoutRedirectUri = process.env.EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI;
  const originalScope = process.env.EXPO_PUBLIC_AUTH0_SCOPE;
  const domain = 'expo-public-auth0-domain';
  const clientId = 'expo-public-auth0-client-id';
  const audience = 'expo-public-auth0-audience';
  const redirectUri = 'expo-public-auth0-redirect-uri';
  const logoutRedirectUri = 'expo-public-auth0-logout-redirect-uri';
  const scope = 'expo-public-auth0-scope';

  beforeEach(() => {
    process.env.EXPO_PUBLIC_AUTH0_DOMAIN = domain;
    process.env.EXPO_PUBLIC_AUTH0_CLIENT_ID = clientId;
    process.env.EXPO_PUBLIC_AUTH0_AUDIENCE = audience;
    process.env.EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI = redirectUri;
    process.env.EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI = logoutRedirectUri;
    process.env.EXPO_PUBLIC_AUTH0_SCOPE = scope;
  });

  afterEach(() => {
    const restoreEnv = (env: string, originalValue?: string) => {
      if (originalValue) {
        process.env[env] = originalValue;
      } else {
        delete process.env[env];
      }
    };

    restoreEnv('EXPO_PUBLIC_AUTH0_DOMAIN', originalDomain);
    restoreEnv('EXPO_PUBLIC_AUTH0_CLIENT_ID', originalClientId);
    restoreEnv('EXPO_PUBLIC_AUTH0_AUDIENCE', originalAudience);
    restoreEnv('EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI', originalRedirectUri);
    restoreEnv('EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI', originalLogoutRedirectUri);
    restoreEnv('EXPO_PUBLIC_AUTH0_SCOPE', originalScope);
  });

  it('succeeds with env values', () => {
    const config = AuthConfig.parseEnv();

    expect(config).toBeSuccessWithValue({
      domain: domain,
      clientId: clientId,
      audience: audience,
      redirectUri: redirectUri,
      logoutRedirectUri: logoutRedirectUri,
      scope: scope,
    });
  });

  it.each([
    'EXPO_PUBLIC_AUTH0_DOMAIN',
    'EXPO_PUBLIC_AUTH0_CLIENT_ID',
    'EXPO_PUBLIC_AUTH0_AUDIENCE',
    'EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI',
    'EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI',
    'EXPO_PUBLIC_AUTH0_SCOPE',
  ])('fails when %s is not set', (envVar: string) => {
    delete process.env[envVar];
    const config = AuthConfig.parseEnv();

    expect(config).toBeFailureWithErrors([`env-variable-${envVar}-not-found`]);
  });
});
