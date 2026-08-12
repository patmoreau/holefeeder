import { AuthenticationState, LoginOptions, TokenInfo } from '@holefeeder/shared/core';
import React, { createContext, useCallback, useMemo } from 'react';
import { Auth0Provider, useAuth0 } from 'react-native-auth0';
import { AuthConfig } from '@/shared/auth/core/auth-config';
import { authorizeParameters } from '@/shared/auth/core/authorize-parameters';
import { DatabaseFactory } from '@/shared/persistence/db';

export const AuthenticationContext = createContext<AuthenticationState | undefined>(undefined);

const InternalAuthenticationProvider = ({ children, config }: { children: React.ReactNode; config: AuthConfig }) => {
  const { user, getCredentials, isLoading, authorize, clearSession } = useAuth0();

  const getToken = async (): Promise<TokenInfo | undefined> => {
    const credentials = await getCredentials();
    return credentials ? { token: credentials.accessToken, expiresAt: credentials.expiresAt } : undefined;
  };

  const login = useCallback(
    async (options?: LoginOptions) => {
      await authorize(authorizeParameters(config, options));
    },
    [authorize, config]
  );

  const logout = useCallback(async () => {
    await DatabaseFactory.instance()?.disconnectAndClear();
    await clearSession({ returnToUrl: config.logoutRedirectUri }, {});
  }, [clearSession, config.logoutRedirectUri]);

  const memoizedUser = useMemo(() => user, [user]);

  const value: AuthenticationState = {
    user: memoizedUser ?? undefined,
    isLoading: isLoading,
    getToken: getToken,
    login: login,
    logout: logout,
  };

  return <AuthenticationContext.Provider value={value}>{children}</AuthenticationContext.Provider>;
};

export const AuthenticationProvider = ({ children, config }: { children: React.ReactNode; config: AuthConfig }) => {
  return (
    <Auth0Provider domain={config.domain} clientId={config.clientId}>
      <InternalAuthenticationProvider config={config}>{children}</InternalAuthenticationProvider>
    </Auth0Provider>
  );
};
