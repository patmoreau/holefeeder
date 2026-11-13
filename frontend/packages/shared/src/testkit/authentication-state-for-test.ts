import { AuthenticationState } from '../core';
import { aTokenInfo } from './token-info-for-test';
import { aUser } from './user-for-test';

const defaultAuthenticationState = (): AuthenticationState => ({
  user: aUser(),
  isLoading: false,
  getToken: () => Promise.resolve(aTokenInfo()),
  login: () => {},
  logout: () => {},
});

export const anAuthenticationState = (overrides?: Partial<AuthenticationState>): AuthenticationState => ({
  ...defaultAuthenticationState(),
  ...overrides,
});
