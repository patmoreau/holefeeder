import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { AuthenticationContextForTest } from '@/shared/auth/__tests__/AuthenticationContextForTest';
import { aUser } from '@/shared/auth/__tests__/user-for-test';
import { AuthenticationState } from '@/shared/auth/core/autentication-state';
import { useAuth } from '@/shared/auth/core/use-auth';

describe('useAuth', () => {
  const user = aUser();

  const createHook = async (overrides?: Partial<AuthenticationState>) =>
    await waitFor(() =>
      renderHook(() => useAuth(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <AuthenticationContextForTest overrides={overrides}>{children}</AuthenticationContextForTest>
        ),
      })
    );

  it('should initialize with loading state', async () => {
    const authenticationState = anAuthenticationState({
      user: undefined,
      isLoading: true,
    });

    const { result } = await createHook(authenticationState);

    expect(result.current.isLoading).toBe(true);
    await waitFor(() => expect(result.current.isLoading).toBeDefined());
  });

  it('should return user ', async () => {
    const authenticationState = anAuthenticationState({
      user: user,
      isLoading: false,
    });

    const { result } = await createHook(authenticationState);

    expect(result.current.isLoading).toBe(false);
    expect(result.current.user).toEqual(user);
  });
});
