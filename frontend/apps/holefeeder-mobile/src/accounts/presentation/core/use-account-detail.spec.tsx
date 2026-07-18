import { Result } from '@holefeeder/shared/core';
import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { anAccountVariation } from '@/accounts/core/__tests__/account-variation-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { aCashflowVariation } from '@/flows/core/flows/__tests__/cashflow-variation-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

const mockUseSettings = jest.fn();
jest.mock('@/shared/presentation/core/use-settings', () => ({
  useSettings: () => mockUseSettings(),
}));

describe('useAccountDetail', () => {
  const account = anAccount();
  let accountRepository: AccountsRepositoryInMemory;
  let flowRepository: FlowsRepositoryInMemory;

  const createHook = async (id = account.id) =>
    renderHook(() => useAccountDetail(id), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ accountRepository, flowRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    accountRepository = AccountsRepositoryInMemory();
    flowRepository = FlowsRepositoryInMemory();
    mockUseSettings.mockReturnValue(Result.success(aSettings()));
  });

  it('emits the account detail when the repositories succeed', async () => {
    accountRepository.add(account);
    flowRepository.addAccountVariations(anAccountVariation({ accountId: account.id }));
    flowRepository.addCashflowVariations(aCashflowVariation({ accountId: account.id }));

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current.isSuccess).toBe(true);
  });

  it('is loading while the account repository is loading', async () => {
    accountRepository.isLoading();

    const { result } = await createHook();

    expect(result.current).toBeLoading();
  });

  it('propagates a repository failure', async () => {
    accountRepository.isFailing(['boom']);

    const { result } = await createHook();

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['boom']);
  });
});
