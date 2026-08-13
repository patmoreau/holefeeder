import { Result } from '@holefeeder/shared/core';
import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { SyncWaitInMilliseconds, useOnboardingRegistration } from '@/user-registration/presentation/core/use-onboarding-registration';
import { Registration, RegistrationContext } from '@/user-registration/presentation/RegistrationProvider';

const mockUseStatus = jest.fn();
jest.mock('@powersync/react', () => ({
  useStatus: () => mockUseStatus(),
}));

describe('useOnboardingRegistration', () => {
  const lastSyncedAt = (date: Date | null) => mockUseStatus.mockReturnValue({ connected: true, lastSyncedAt: date, dataFlowStatus: {} });

  const aRegistration = (overrides?: Partial<Registration>): Registration => ({
    status: Result.success(RegistrationStatuses.notRegistered),
    recheck: jest.fn(),
    registration: Result.loading(),
    register: jest.fn(),
    ...overrides,
  });

  // The hook sets state from an effect on first render, so the render itself has to
  // happen inside act or the update lands outside one and result.current reads null.
  const createHook = async (registration: Registration) => {
    let hook: ReturnType<typeof renderHook<ReturnType<typeof useOnboardingRegistration>, unknown>> | undefined;
    await act(async () => {
      hook = renderHook(() => useOnboardingRegistration(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <RegistrationContext.Provider value={registration}>{children}</RegistrationContext.Provider>
        ),
      });
    });
    return hook!;
  };

  beforeEach(() => {
    mockUseStatus.mockReset();
    lastSyncedAt(null);
  });

  it('registers the caller as soon as the screen appears', async () => {
    const registration = aRegistration();

    await createHook(registration);

    await waitFor(() => expect(registration.register).toHaveBeenCalledTimes(1));
  });

  it('registers only once across re-renders', async () => {
    const registration = aRegistration();

    const { rerender } = await createHook(registration);
    await act(async () => rerender({}));
    await act(async () => rerender({}));

    await waitFor(() => expect(registration.register).toHaveBeenCalledTimes(1));
  });

  it('keeps showing progress while registering', async () => {
    const { result } = await createHook(aRegistration());

    expect(result.current.progress).toBeLoading();
  });

  it('surfaces a registration failure so the screen can offer a retry', async () => {
    const registration = aRegistration({ registration: Result.failure(['server-error']) });

    const { result } = await createHook(registration);

    await waitFor(() => expect(result.current.progress.isFailure).toBe(true));
  });

  it('keeps waiting after registering until data has synced', async () => {
    const registration = aRegistration({ registration: Result.success() });

    const { result } = await createHook(registration);

    await waitFor(() => expect(registration.register).toHaveBeenCalled());
    expect(result.current.progress).toBeLoading();
    expect(result.current.ready).toBe(false);
  });

  it('ignores a sync that finished before registering', async () => {
    lastSyncedAt(new Date('2020-01-01T00:00:00Z'));
    const registration = aRegistration({ registration: Result.success() });

    const hook = await createHook(registration);

    const { result } = hook;
    await waitFor(() => expect(registration.register).toHaveBeenCalled());
    // Before the cap elapses, an older sync must not count as this user's data.
    expect(result.current.ready).toBe(false);
  });

  it('goes in anyway when the first sync never arrives', async () => {
    jest.useFakeTimers();
    try {
      const registration = aRegistration({ registration: Result.success() });

      const { result, rerender } = await createHook(registration);
      await waitFor(() => expect(registration.register).toHaveBeenCalled());
      expect(result.current.ready).toBe(false);

      await act(async () => {
        jest.advanceTimersByTime(SyncWaitInMilliseconds);
      });
      await act(async () => rerender({}));

      expect(result.current.ready).toBe(true);
    } finally {
      jest.useRealTimers();
    }
  });

  it('is ready once data has synced', async () => {
    const registration = aRegistration({ registration: Result.success() });

    const { result, rerender } = await createHook(registration);
    await waitFor(() => expect(registration.register).toHaveBeenCalled());

    lastSyncedAt(new Date(Date.now() + 1000));
    await act(async () => rerender({}));

    await waitFor(() => expect(result.current.ready).toBe(true));
  });

  it('never opens the gate itself — onboarding is not finished here', async () => {
    const registration = aRegistration({ registration: Result.success() });

    const { rerender } = await createHook(registration);
    lastSyncedAt(new Date(Date.now() + 1000));
    await act(async () => rerender({}));

    expect(registration.recheck).not.toHaveBeenCalled();
  });
});
