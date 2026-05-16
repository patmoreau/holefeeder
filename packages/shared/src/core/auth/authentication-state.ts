import { TokenInfo } from './token-info';
import { User } from './user';

export type AuthenticationState = {
  user: User | undefined;
  isLoading: boolean;
  getToken: () => Promise<TokenInfo | undefined>;
  login: () => void;
  logout: () => void;
};
