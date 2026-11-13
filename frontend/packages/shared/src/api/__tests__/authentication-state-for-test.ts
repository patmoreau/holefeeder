import { AuthenticationState, TokenInfo } from '../../core';
import { aString, aWord } from '../../core/__tests__/helpers/string-for-test';

export const aTokenInfo = (overrides?: Partial<TokenInfo>): TokenInfo => ({
  token: aWord(),
  expiresAt: Date.now() + 3_600_000,
  ...overrides,
});

export const anAuthenticationState = (overrides?: Partial<AuthenticationState>): AuthenticationState => ({
  user: { sub: aWord(), email: `${aWord()}@example.com`, name: aString() },
  isLoading: false,
  getToken: () => Promise.resolve(aTokenInfo()),
  login: () => {},
  logout: () => {},
  ...overrides,
});
