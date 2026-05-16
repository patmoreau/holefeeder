import { Platform } from 'react-native';
import { EnvVariable } from '@/shared/core/env/env-variable';
import { Result } from '@/shared/core/result';

export type AuthConfig = {
  domain: string;
  clientId: string;
  audience: string;
  redirectUri: string;
  logoutRedirectUri: string;
  scope: string;
};

const parseEnv = (): Result<AuthConfig> => {
  return Result.combine({
    domain: EnvVariable.read('EXPO_PUBLIC_AUTH0_DOMAIN'),
    clientId: EnvVariable.read('EXPO_PUBLIC_AUTH0_CLIENT_ID'),
    audience: EnvVariable.read('EXPO_PUBLIC_AUTH0_AUDIENCE'),
    redirectUri: parseRedirectUriEnv(),
    logoutRedirectUri: parseLogoutRedirectUriEnv(),
    scope: EnvVariable.read('EXPO_PUBLIC_AUTH0_SCOPE'),
  });
};

const parseRedirectUriEnv = (): Result<string> => {
  return Platform.OS === 'web'
    ? EnvVariable.read('EXPO_PUBLIC_AUTH0_WEB_REDIRECT_URI')
    : Platform.OS === 'android'
      ? EnvVariable.read('EXPO_PUBLIC_AUTH0_ANDROID_REDIRECT_URI')
      : EnvVariable.read('EXPO_PUBLIC_AUTH0_IOS_REDIRECT_URI');
};

const parseLogoutRedirectUriEnv = (): Result<string> => {
  return Platform.OS === 'web'
    ? EnvVariable.read('EXPO_PUBLIC_AUTH0_WEB_LOGOUT_REDIRECT_URI')
    : Platform.OS === 'android'
      ? EnvVariable.read('EXPO_PUBLIC_AUTH0_ANDROID_LOGOUT_REDIRECT_URI')
      : EnvVariable.read('EXPO_PUBLIC_AUTH0_IOS_LOGOUT_REDIRECT_URI');
};

export const AuthConfig = {
  parseEnv: parseEnv,
} as const;
