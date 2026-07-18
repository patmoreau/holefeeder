import { renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { useTransactions } from '@/accounts/presentation/core/use-transactions';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTransaction, toTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { anId } from '@/shared/__tests__/string-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

describe('useTransactions', () => {
  const accountId = anId();
  let flowRepository: FlowsRepositoryInMemory;

  const createHook = async () =>
    renderHook(() => useTransactions(accountId), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ flowRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    flowRepository = FlowsRepositoryInMemory();
  });

  it('emits the account transactions and total count on success', async () => {
    const transactions = [toTransaction(aTransaction({ accountId })), toTransaction(aTransaction({ accountId }))];
    flowRepository.addTransactions(...transactions);

    const { result } = await createHook();

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue(transactions);
    expect(result.current.totalCount).toBe(2);
    expect(result.current.hasNextPage).toBe(false);
    expect(result.current.hasPreviousPage).toBe(false);
  });

  it('emits an empty success when the account has no transactions', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue([]);
    expect(result.current.totalCount).toBe(0);
  });

  it('stays loading while the repository is loading', async () => {
    flowRepository.isLoading();

    const { result } = await createHook();

    expect(result.current.transactions).toBeLoading();
  });

  it('stays loading when the repository fails (failures are swallowed)', async () => {
    flowRepository.isFailing(['boom']);

    const { result } = await createHook();

    // useTransactions only forwards successful pages, so a repository
    // failure never surfaces — the hook remains in the loading state.
    expect(result.current.transactions).toBeLoading();
  });
});
