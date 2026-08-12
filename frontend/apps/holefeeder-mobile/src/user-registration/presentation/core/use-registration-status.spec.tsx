import { AuthenticationState, buildUrl } from '@holefeeder/shared/core';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anApiConfig } from '@/shared/api/__tests__/api-config-for-test';
import { FetchForTest } from '@/shared/api/__tests__/fetch-for-test';
import { aFetchRequest } from '@/shared/api/__tests__/fetch-request-for-test';
import { aFetchResponse } from '@/shared/api/__tests__/fetch-response-for-test';
import { anAuthenticationState } from '@/shared/auth/__tests__/authentication-state-for-test';
import { AuthenticationContext } from '@/shared/auth/presentation/AuthenticationProvider';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { useRegistrationStatus } from '@/user-registration/presentation/core/use-registration-status';

describe('useRegistrationStatus', () => {
  const apiConfig = anApiConfig();
  const url = buildUrl(apiConfig.url, '/api/v2/users/me');

  let fetchForTest: FetchForTest;

  const createHook = async (authenticationState: AuthenticationState) =>
    renderHook(() => useRegistrationStatus(apiConfig), {
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

  it('reports a known caller as registered', async () => {
    respondWith(200, { id: 'c4b9e2a1-7d63-4f58-92ac-1e0d5b8f3a70' });

    const { result } = await createHook(anAuthenticationState());

    await waitFor(() => expect(result.current.status).not.toBeLoading());
    expect(result.current.status).toBeSuccessWithValue(RegistrationStatuses.registered);
  });

  it('reports an unknown caller as not registered', async () => {
    respondWith(404);

    const { result } = await createHook(anAuthenticationState());

    await waitFor(() => expect(result.current.status).not.toBeLoading());
    expect(result.current.status).toBeSuccessWithValue(RegistrationStatuses.notRegistered);
  });

  it('fails rather than guessing when the server errors', async () => {
    respondWith(500);

    const { result } = await createHook(anAuthenticationState());

    await waitFor(() => expect(result.current.status).not.toBeLoading());
    expect(result.current.status.isFailure).toBe(true);
  });

  it('stays loading while there is no signed-in user', async () => {
    respondWith(200, { id: 'c4b9e2a1-7d63-4f58-92ac-1e0d5b8f3a70' });

    const { result } = await createHook(anAuthenticationState({ user: undefined }));

    expect(result.current.status).toBeLoading();
  });

  it('asks again when rechecked', async () => {
    respondWith(500);

    const { result } = await createHook(anAuthenticationState());

    await waitFor(() => expect(result.current.status).not.toBeLoading());
    expect(result.current.status.isFailure).toBe(true);

    respondWith(200, { id: 'c4b9e2a1-7d63-4f58-92ac-1e0d5b8f3a70' });
    await act(async () => {
      result.current.recheck();
    });

    await waitFor(() => expect(result.current.status).not.toBeLoading());
    expect(result.current.status).toBeSuccessWithValue(RegistrationStatuses.registered);
  });
});
