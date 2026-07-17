import { type AsyncResult } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTransaction, toTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { Transaction } from '@/flows/core/flows/transaction';
import { WatchFlowUseCase } from '@/flows/core/flows/watch-flow/watch-flow-use-case';

describe('WatchFlowUseCase', () => {
  let repository: FlowsRepositoryInMemory;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
  });

  it('returns the transaction matching the id when the repository succeeds', async () => {
    const transaction = toTransaction(aTransaction());
    repository.addTransactions(transaction);
    const useCase = WatchFlowUseCase(transaction.id, repository);

    let result: AsyncResult<Transaction> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());
    expect(result).toBeSuccessWithValue(transaction);

    unsubscribe();
  });

  it('returns failure when the repository fails', async () => {
    const transaction = toTransaction(aTransaction());
    repository.addTransactions(transaction);
    repository.isFailing(['error']);
    const useCase = WatchFlowUseCase(transaction.id, repository);

    let result: AsyncResult<Transaction> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());
    expect(result).toBeFailureWithErrors(['error']);

    unsubscribe();
  });
});
