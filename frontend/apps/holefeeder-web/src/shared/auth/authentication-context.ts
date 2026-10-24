import { type AuthenticationState } from '@holefeeder/shared/core';
import { createContext, useContext } from 'react';

export const AuthenticationContext = createContext<AuthenticationState | null>(null);

export const useAuthentication = (): AuthenticationState => {
  const ctx = useContext(AuthenticationContext);
  if (!ctx) throw new Error('useAuthentication must be used within AuthenticationProvider');
  return ctx;
};
