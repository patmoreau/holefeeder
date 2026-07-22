import { act, renderHook, waitFor } from '@testing-library/react-native';
import React from 'react';
import { useAccountTransactions } from '@/accounts/presentation/core/use-account-transactions';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTransaction, toTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { anId } from '@/shared/__tests__/string-for-test';
import { RepositoryContextForTest } from '@/shared/repositories/__tests__/RepositoryContextForTest';

describe('useAccountTransactions', () => {
  const accountId = anId();
  let flowRepository: FlowsRepositoryInMemory;

  const createHook = async (pageSize?: number) =>
    renderHook(() => useAccountTransactions(accountId, pageSize), {
      wrapper: ({ children }: { children: React.ReactNode }) => (
        <RepositoryContextForTest repositories={{ flowRepository }}>{children}</RepositoryContextForTest>
      ),
    });

  beforeEach(() => {
    flowRepository = FlowsRepositoryInMemory();
  });

  it('emits the transactions for the account on success', async () => {
    const transactions = [toTransaction(aTransaction({ accountId })), toTransaction(aTransaction({ accountId }))];
    flowRepository.addTransactions(...transactions);

    const { result } = await createHook();

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue(transactions);
    expect(result.current.hasMore).toBe(false);
  });

  it('excludes transactions from other accounts', async () => {
    const mine = toTransaction(aTransaction({ accountId }));
    flowRepository.addTransactions(mine, toTransaction(aTransaction({ accountId: anId() })));

    const { result } = await createHook();

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue([mine]);
  });

  it('emits an empty success when the account has no transactions', async () => {
    const { result } = await createHook();

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue([]);
    expect(result.current.hasMore).toBe(false);
  });

  it('limits the emitted transactions to the page size and reports more available', async () => {
    const transactions = aTransaction({ accountId }).times(3).map(toTransaction);
    flowRepository.addTransactions(...transactions);

    const { result } = await createHook(2);

    await waitFor(() => expect(result.current.transactions).not.toBeLoading());

    expect(result.current.transactions).toBeSuccessWithValue(transactions.slice(0, 2));
    expect(result.current.hasMore).toBe(true);
  });

  it('grows the window and clears hasMore when loadMore is called', async () => {
    const transactions = aTransaction({ accountId }).times(3).map(toTransaction);
    flowRepository.addTransactions(...transactions);

    const { result } = await createHook(2);

    await waitFor(() => expect(result.current.hasMore).toBe(true));

    await act(async () => {
      result.current.loadMore();
    });

    await waitFor(() => expect(result.current.hasMore).toBe(false));
    expect(result.current.transactions).toBeSuccessWithValue(transactions);
  });

  it('stays loading while the repository is loading', async () => {
    flowRepository.isLoading();

    const { result } = await createHook();

    expect(result.current.transactions).toBeLoading();
  });
});
