import { Result } from '@holefeeder/shared/core';
import { act, renderHook } from '@testing-library/react-native';
import React from 'react';
import { CategoriesRepositoryInMemory } from '@/flows/core/categories/__tests__/categories-repository-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { SuggestedCategoryKeys } from '@/user-registration/core/suggested-categories';
import { useOnboardingCategories } from '@/user-registration/presentation/core/use-onboarding-categories';
import { Registration, RegistrationContext } from '@/user-registration/presentation/RegistrationProvider';

describe('useOnboardingCategories', () => {
  let categoryRepository: CategoriesRepositoryInMemory;

  const aRegistration = (overrides?: Partial<Registration>): Registration => ({
    status: Result.success(RegistrationStatuses.notRegistered),
    recheck: jest.fn(),
    registration: Result.success(),
    register: jest.fn(),
    ...overrides,
  });

  const createHook = async (registration: Registration) =>
    renderHook(() => useOnboardingCategories(), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ categoryRepository }}>
          <RegistrationContext.Provider value={registration}>{children}</RegistrationContext.Provider>
        </RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    categoryRepository = CategoriesRepositoryInMemory();
  });

  it('offers every suggestion, selected to begin with', async () => {
    const { result } = await createHook(aRegistration());

    expect(result.current.choices).toHaveLength(SuggestedCategoryKeys.length);
    expect(result.current.choices.every((choice) => choice.selected)).toBe(true);
  });

  it('creates the selected categories and lets the caller into the app', async () => {
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      await result.current.finish();
    });

    expect(categoryRepository.createdCommands()).toHaveLength(SuggestedCategoryKeys.length);
    expect(registration.recheck).toHaveBeenCalledTimes(1);
  });

  it('leaves out what the caller deselected', async () => {
    const { result } = await createHook(aRegistration());

    await act(async () => {
      result.current.toggle('groceries');
      result.current.toggle('housing');
    });
    await act(async () => {
      await result.current.finish();
    });

    const created = categoryRepository.createdCommands().map((command) => command.name);
    expect(created).toHaveLength(SuggestedCategoryKeys.length - 2);
    expect(created).not.toContain('Groceries');
    expect(created).not.toContain('Housing');
  });

  it('creates categories as expenses the user owns outright', async () => {
    const { result } = await createHook(aRegistration());

    await act(async () => {
      await result.current.finish();
    });

    for (const command of categoryRepository.createdCommands()) {
      expect(command.type).toBe('expense');
      expect(command.favorite).toBe(false);
      expect(command.color).toMatch(/^#[0-9A-F]{6}$/i);
    }
  });

  it('lets the caller in without creating anything when they skip', async () => {
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      result.current.skip();
    });

    expect(categoryRepository.createdCommands()).toHaveLength(0);
    expect(registration.recheck).toHaveBeenCalledTimes(1);
  });

  it('keeps the caller on the step when a category cannot be created', async () => {
    categoryRepository.isFailing(['boom']);
    const registration = aRegistration();

    const { result } = await createHook(registration);
    await act(async () => {
      await result.current.finish();
    });

    expect(result.current.failed).toBe(true);
    expect(registration.recheck).not.toHaveBeenCalled();
  });
});
