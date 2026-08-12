import { TokenInfo } from './token-info';
import { User } from './user';

// Signing in and signing up are the same call to the identity provider; only the
// page it opens on differs.
export type LoginOptions = {
  signUp?: boolean;
};

export type AuthenticationState = {
  user: User | undefined;
  isLoading: boolean;
  getToken: () => Promise<TokenInfo | undefined>;
  login: (options?: LoginOptions) => void;
  logout: () => void;
};
