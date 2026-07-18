import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { useAccount } from '@/accounts/presentation/core/use-account';
import { anId } from '@/shared/__tests__/string-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

describe('useAccount', () => {
  const account = anAccount();
  let accountRepository: AccountsRepositoryInMemory;

  const createHook = async (id = account.id) =>
    renderHook(() => useAccount(id), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ accountRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    accountRepository = AccountsRepositoryInMemory();
    accountRepository.add(account);
  });

  it('returns the account matching the id', async () => {
    const { result } = await createHook(account.id);

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeSuccessWithValue(account);
  });

  it('fails with account-not-found when no account matches the id', async () => {
    const { result } = await createHook(anId());

    await waitFor(() => expect(result.current).not.toBeLoading());

    expect(result.current).toBeFailureWithErrors(['account-not-found']);
  });

  it('is loading while the repository is loading', async () => {
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
