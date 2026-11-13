import { AuthenticationState } from '@holefeeder/shared/core';
import { anAuthenticationState } from '@holefeeder/shared/testkit';
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
