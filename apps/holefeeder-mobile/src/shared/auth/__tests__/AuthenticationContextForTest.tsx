import { AuthenticationState } from '@holefeeder/core';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
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
