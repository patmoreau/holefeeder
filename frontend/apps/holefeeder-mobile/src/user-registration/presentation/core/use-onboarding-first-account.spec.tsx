import { Result } from '@holefeeder/shared/core';
import { act, renderHook } from '@testing-library/react-native';
import { router } from 'expo-router';
import React from 'react';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { useOnboardingFirstAccount } from '@/user-registration/presentation/core/use-onboarding-first-account';
import { Registration, RegistrationContext } from '@/user-registration/presentation/RegistrationProvider';

jest.mock('expo-router', () => ({ router: { replace: jest.fn() } }));

const mockSaveForm = jest.fn();
jest.mock('@/accounts/presentation/core/use-edit-account-form', () => ({
  useEditAccountForm: () => ({ saveForm: mockSaveForm }),
}));

describe('useOnboardingFirstAccount', () => {
  const aRegistration = (overrides?: Partial<Registration>): Registration => ({
    status: Result.success(RegistrationStatuses.notRegistered),
    recheck: jest.fn(),
    registration: Result.success(),
    register: jest.fn(),
    ...overrides,
  });

  const createHook = async (registration: Registration) =>
    renderHook(() => useOnboardingFirstAccount(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RegistrationContext.Provider value={registration}>{children}</RegistrationContext.Provider>
      ),
    });

  beforeEach(() => {
    mockSaveForm.mockReset();
    (router.replace as jest.Mock).mockReset();
  });

  it('saves the account and moves on to the categories', async () => {
    mockSaveForm.mockResolvedValue(true);
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      await result.current.finish();
    });

    expect(mockSaveForm).toHaveBeenCalledTimes(1);
    expect(router.replace).toHaveBeenCalledWith('/Categories');
    expect(registration.recheck).not.toHaveBeenCalled();
  });

  it('keeps the caller on the step when the form will not save', async () => {
    mockSaveForm.mockResolvedValue(false);

    const { result } = await createHook(aRegistration());
    await act(async () => {
      await result.current.finish();
    });

    expect(router.replace).not.toHaveBeenCalled();
  });

  it('reports saving so the button can be disabled', async () => {
    let release: (saved: boolean) => void = () => {};
    mockSaveForm.mockReturnValue(new Promise<boolean>((resolve) => (release = resolve)));
    const { result } = await createHook(aRegistration());

    let finished: Promise<void> | undefined;
    await act(async () => {
      finished = result.current.finish();
    });
    expect(result.current.isSaving).toBe(true);

    await act(async () => {
      release(true);
      await finished;
    });
    expect(result.current.isSaving).toBe(false);
  });
});
