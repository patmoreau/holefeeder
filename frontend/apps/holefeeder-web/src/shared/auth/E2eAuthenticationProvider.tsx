import { type AuthenticationState, type LoginOptions, type User } from '@holefeeder/shared/core';
import { type ReactNode } from 'react';
import { AuthenticationContext } from './authentication-context';

type E2eWindow = Window & {
  __E2E_AUTH__?: { user?: User; isLoading?: boolean };
  __E2E_LOGIN_CALLS__?: LoginOptions[];
};

export const E2eAuthenticationProvider = ({ children }: { children: ReactNode }) => {
  const e2eWindow = window as E2eWindow;
  const arranged = e2eWindow.__E2E_AUTH__ ?? {};

  const state: AuthenticationState = {
    user: arranged.user,
    isLoading: arranged.isLoading ?? false,
    getToken: async () => (arranged.user ? { token: 'e2e-token', expiresAt: Number.MAX_SAFE_INTEGER } : undefined),
    login: (options) => {
      e2eWindow.__E2E_LOGIN_CALLS__ = [...(e2eWindow.__E2E_LOGIN_CALLS__ ?? []), { signUp: options?.signUp ?? false }];
    },
    logout: () => {},
  };

  return <AuthenticationContext.Provider value={state}>{children}</AuthenticationContext.Provider>;
};
