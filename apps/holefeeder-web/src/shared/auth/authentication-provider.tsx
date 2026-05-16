import { Auth0Provider, useAuth0 } from '@auth0/auth0-react';
import { createContext, useContext, type ReactNode } from 'react';
import { type AuthenticationState, type TokenInfo } from '@holefeeder/core';

const AuthContext = createContext<AuthenticationState | null>(null);

const AuthStateProvider = ({ children }: { children: ReactNode }) => {
  const { user, isLoading, getAccessTokenSilently, loginWithRedirect, logout } = useAuth0();

  const getToken = async (): Promise<TokenInfo | undefined> => {
    try {
      const token = await getAccessTokenSilently();
      const payload = JSON.parse(atob(token.split('.')[1])) as { exp: number };
      return { token, expiresAt: payload.exp };
    } catch {
      return undefined;
    }
  };

  const state: AuthenticationState = {
    user: user
      ? {
          sub: user.sub ?? '',
          name: user.name,
          givenName: user.given_name,
          familyName: user.family_name,
          picture: user.picture,
          email: user.email,
        }
      : undefined,
    isLoading,
    getToken,
    login: () => void loginWithRedirect(),
    logout: () => void logout({ logoutParams: { returnTo: window.location.origin } }),
  };

  return <AuthContext.Provider value={state}>{children}</AuthContext.Provider>;
};

export const AuthenticationProvider = ({ children }: { children: ReactNode }) => (
  <Auth0Provider
    domain={import.meta.env.VITE_AUTH0_DOMAIN as string}
    clientId={import.meta.env.VITE_AUTH0_CLIENT_ID as string}
    authorizationParams={{
      redirect_uri: window.location.origin,
      audience: import.meta.env.VITE_AUTH0_AUDIENCE as string,
    }}
  >
    <AuthStateProvider>{children}</AuthStateProvider>
  </Auth0Provider>
);

export const useAuthentication = (): AuthenticationState => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuthentication must be used within AuthenticationProvider');
  return ctx;
};
