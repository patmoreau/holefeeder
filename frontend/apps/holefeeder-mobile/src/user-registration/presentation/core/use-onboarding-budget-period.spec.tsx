import { Result } from '@holefeeder/shared/core';
import { act, renderHook } from '@testing-library/react-native';
import React from 'react';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { useOnboardingBudgetPeriod } from '@/user-registration/presentation/core/use-onboarding-budget-period';
import { Registration, RegistrationContext } from '@/user-registration/presentation/RegistrationProvider';

const mockSaveForm = jest.fn();
jest.mock('@/settings/presentation/core/use-settings-form', () => ({
  useSettingsForm: () => ({ saveForm: mockSaveForm }),
}));

describe('useOnboardingBudgetPeriod', () => {
  const aRegistration = (overrides?: Partial<Registration>): Registration => ({
    status: Result.success(RegistrationStatuses.notRegistered),
    recheck: jest.fn(),
    registration: Result.success(),
    register: jest.fn(),
    ...overrides,
  });

  const createHook = async (registration: Registration) =>
    renderHook(() => useOnboardingBudgetPeriod(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RegistrationContext.Provider value={registration}>{children}</RegistrationContext.Provider>
      ),
    });

  beforeEach(() => {
    mockSaveForm.mockReset();
  });

  it('saves the budget period and lets the caller into the app', async () => {
    mockSaveForm.mockResolvedValue(true);
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      await result.current.finish();
    });

    expect(mockSaveForm).toHaveBeenCalledTimes(1);
    expect(registration.recheck).toHaveBeenCalledTimes(1);
  });

  it('keeps the caller on the step when the form will not save', async () => {
    mockSaveForm.mockResolvedValue(false);
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      await result.current.finish();
    });

    expect(registration.recheck).not.toHaveBeenCalled();
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
