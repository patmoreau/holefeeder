import { AuthenticationState, buildUrl } from '@holefeeder/shared/core';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anApiConfig } from '@/shared/api/__tests__/api-config-for-test';
import { FetchForTest } from '@/shared/api/__tests__/fetch-for-test';
import { aFetchRequest } from '@/shared/api/__tests__/fetch-request-for-test';
import { aFetchResponse } from '@/shared/api/__tests__/fetch-response-for-test';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { AuthenticationContext } from '@/shared/auth/presentation/AuthenticationProvider';
import { useRegisterUser } from '@/user-registration/presentation/core/use-register-user';

describe('useRegisterUser', () => {
  const apiConfig = anApiConfig();
  const url = buildUrl(apiConfig.url, '/api/v2/users/register');

  let fetchForTest: FetchForTest;

  const createHook = async (authenticationState: AuthenticationState = anAuthenticationState()) =>
    renderHook(() => useRegisterUser(apiConfig), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <AuthenticationContext.Provider value={authenticationState}>{children}</AuthenticationContext.Provider>
      ),
    });

  const respondWith = (status: number, body: unknown = {}) => {
    const serialized = JSON.stringify(body);
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');
    headers.append('Content-Length', serialized.length.toString());
    fetchForTest.simulate({
      request: aFetchRequest({ url, matchHeaders: false }),
      response: aFetchResponse({ body: serialized, status: status, ok: status < 400, headers: headers }),
    });
  };

  beforeEach(() => {
    fetchForTest = FetchForTest();
  });

  afterEach(() => {
    fetchForTest.restore();
  });

  it('is loading before anything is asked of it', async () => {
    respondWith(200, { id: 'e0b6a1c4-3f92-4d78-8b5a-6c2d9e174f38' });

    const { result } = await createHook();

    expect(result.current.registration).toBeLoading();
  });

  it('succeeds when the caller is registered', async () => {
    respondWith(200, { id: 'e0b6a1c4-3f92-4d78-8b5a-6c2d9e174f38' });

    const { result } = await createHook();
    await act(async () => {
      result.current.register();
    });

    await waitFor(() => expect(result.current.registration).not.toBeLoading());
    expect(result.current.registration.isSuccess).toBe(true);
  });

  it('succeeds when the caller was already registered', async () => {
    respondWith(400);

    const { result } = await createHook();
    await act(async () => {
      result.current.register();
    });

    await waitFor(() => expect(result.current.registration).not.toBeLoading());
    expect(result.current.registration.isSuccess).toBe(true);
  });

  it('fails when the server errors', async () => {
    respondWith(500);

    const { result } = await createHook();
    await act(async () => {
      result.current.register();
    });

    await waitFor(() => expect(result.current.registration).not.toBeLoading());
    expect(result.current.registration.isFailure).toBe(true);
  });

  it('can be retried after a failure', async () => {
    respondWith(500);

    const { result } = await createHook();
    await act(async () => {
      result.current.register();
    });
    await waitFor(() => expect(result.current.registration.isFailure).toBe(true));

    respondWith(200, { id: 'e0b6a1c4-3f92-4d78-8b5a-6c2d9e174f38' });
    await act(async () => {
      result.current.register();
    });

    await waitFor(() => expect(result.current.registration).not.toBeLoading());
    expect(result.current.registration.isSuccess).toBe(true);
  });
});
