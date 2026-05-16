import { aTokenInfo } from '@/shared/auth/__tests__/token-info-for-test';
import { aUser } from '@/shared/auth/__tests__/user-for-test';
import { AuthenticationState } from '@/shared/auth/core/autentication-state';

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
