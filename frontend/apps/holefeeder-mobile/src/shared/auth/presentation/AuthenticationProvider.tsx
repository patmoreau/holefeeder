import { AuthenticationState, Logger, LoginOptions, TokenInfo } from '@holefeeder/shared/core';
import React, { createContext, useCallback, useMemo } from 'react';
import { Auth0Provider, useAuth0 } from 'react-native-auth0';
import { AuthConfig } from '@/shared/auth/core/auth-config';
import { authorizeParameters } from '@/shared/auth/core/authorize-parameters';
import { isUserCancellation } from '@/shared/auth/core/is-user-cancellation';
import { DatabaseFactory } from '@/shared/persistence/db';

export const AuthenticationContext = createContext<AuthenticationState | undefined>(undefined);

const logger = Logger.create('AuthenticationProvider');

const InternalAuthenticationProvider = ({ children, config }: { children: React.ReactNode; config: AuthConfig }) => {
  const { user, getCredentials, isLoading, authorize, clearSession } = useAuth0();

  const getToken = async (): Promise<TokenInfo | undefined> => {
    const credentials = await getCredentials();
    return credentials ? { token: credentials.accessToken, expiresAt: credentials.expiresAt } : undefined;
  };

  const login = useCallback(
    async (options?: LoginOptions) => {
      try {
        await authorize(authorizeParameters(config, options));
      } catch (error) {
        // Nothing calls login() for its result — it is fired from an onPress — so a
        // rejection here surfaces as an unhandled promise rejection. Backing out of
        // the browser sheet is not a failure and must not read like one.
        if (isUserCancellation(error)) {
          logger.info('Sign-in cancelled by the user');
          return;
        }
        logger.error('Sign-in failed', error);
      }
    },
    [authorize, config]
  );

  const logout = useCallback(async () => {
    await DatabaseFactory.instance()?.disconnectAndClear();
    try {
      await clearSession({ returnToUrl: config.logoutRedirectUri }, {});
    } catch (error) {
      // Same story as login: the sign-out sheet can be dismissed, and that is not an
      // error either.
      if (isUserCancellation(error)) {
        logger.info('Sign-out cancelled by the user');
        return;
      }
      logger.error('Sign-out failed', error);
    }
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
