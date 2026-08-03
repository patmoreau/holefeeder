import { Result } from '@holefeeder/shared/core';
import { aCashflow, toCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { WatchCashflowsUseCase } from '@/flows/core/flows/watch-cashflows/watch-cashflows-use-case';

describe('WatchCashflowsUseCase', () => {
  it('emits the active cashflows from the repository', () => {
    const repository = FlowsRepositoryInMemory();
    const cashflow = toCashflow(aCashflow());
    repository.addCashflows(cashflow);

    let received: unknown;
    const unsubscribe = WatchCashflowsUseCase(repository).watch((result) => {
      received = result;
    });

    expect(received).toEqual(Result.success([cashflow]));

    unsubscribe();
  });
});
