import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { useAccounts } from '@/flows/presentation/shared/core/use-accounts';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

describe('useAccounts', () => {
  const account = anAccount();
  let accountRepository: AccountsRepositoryInMemory;

  const createHook = async (accountRepository: AccountsRepository) =>
    await waitFor(() =>
      renderHook(() => useAccounts(), {
        wrapper: ({ children }: { children: React.ReactNode }) => (
          <RepositoryContextForTest repositories={{ accountRepository: accountRepository }}>{children}</RepositoryContextForTest>
        ),
      })
    );

  beforeEach(() => {
    accountRepository = AccountsRepositoryInMemory();
    accountRepository.add(account);
  });

  it('fetches accounts', async () => {
    const { result } = await createHook(accountRepository);

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue([account]);
  });

  it('waits for accounts', async () => {
    accountRepository.isLoading();
    const { result } = await createHook(accountRepository);

    expect(result.current).toBeLoading();
  });

  it('handles errors', async () => {
    accountRepository.isFailing(['test-error']);
    const { result } = await createHook(accountRepository);

    expect(result.current).toBeFailureWithErrors(['test-error']);
  });
});
