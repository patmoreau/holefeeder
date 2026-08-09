import { AuthenticationState, Logger, TokenInfo } from '@holefeeder/shared/core';
import * as Linking from 'expo-linking';
import React, { useCallback, useEffect, useState } from 'react';
import { E2eSession, E2eSessionData } from '@/shared/auth/core/e2e-session';
import { AuthenticationContext } from '@/shared/auth/presentation/AuthenticationProvider';
import { DatabaseFactory } from '@/shared/persistence/db';

const logger = Logger.create('E2eAuthenticationProvider');

// Stands in for the Auth0 provider in E2E builds only: it never opens a browser,
// taking its session from a holefeeder://e2e-auth link instead. Mounted solely
// when E2eConfig.isEnabled(), which a production build cannot satisfy.
export const E2eAuthenticationProvider = ({ children }: { children: React.ReactNode }) => {
  const [session, setSession] = useState<E2eSessionData | undefined>(undefined);

  useEffect(() => {
    const accept = (url: string | null) => {
      if (!url) return;

      const result = E2eSession.parseLink(url);
      if (result.isFailure) {
        logger.warn('Ignoring link', url, result.errors);
        return;
      }

      logger.warn('Accepted an E2E session for', result.value.user.sub);
      setSession(result.value);
    };

    // Both halves are needed: getInitialURL covers a cold start from the link,
    // addEventListener covers a link delivered to the running app.
    Linking.getInitialURL().then(accept);
    const subscription = Linking.addEventListener('url', (event) => accept(event.url));
    return () => subscription.remove();
  }, []);

  const getToken = useCallback(
    async (): Promise<TokenInfo | undefined> => (session ? { token: session.token, expiresAt: session.expiresAt } : undefined),
    [session]
  );

  const login = useCallback(() => {
    logger.warn('login() is a no-op in E2E builds — send a holefeeder://e2e-auth link instead');
  }, []);

  const logout = useCallback(async () => {
    await DatabaseFactory.instance()?.disconnectAndClear();
    setSession(undefined);
  }, []);

  const value: AuthenticationState = {
    user: session?.user,
    isLoading: false,
    getToken: getToken,
    login: login,
    logout: logout,
  };

  return <AuthenticationContext.Provider value={value}>{children}</AuthenticationContext.Provider>;
};
