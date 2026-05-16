import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { AuthenticationState } from '@/shared/auth/core/autentication-state';
import { AuthenticationContext } from '@/shared/auth/presentation/AuthenticationProvider';

export const AuthenticationContextForTest = ({
  children,
  overrides,
}: {
  children: React.ReactNode;
  overrides?: Partial<AuthenticationState>;
}) => {
  return <AuthenticationContext.Provider value={anAuthenticationState(overrides)}>{children}</AuthenticationContext.Provider>;
};
