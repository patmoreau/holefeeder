import { Result } from '@holefeeder/shared/core';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aModifyCashflowCommand } from '@/flows/core/flows/modify-cashflow/__tests__/modify-cashflow-command-for-test';
import { ModifyCashflowUseCase } from '@/flows/core/flows/modify-cashflow/modify-cashflow-use-case';

describe('ModifyCashflowUseCase', () => {
  it('delegates to the repository and returns its result', async () => {
    const repository = FlowsRepositoryInMemory();
    const command = aModifyCashflowCommand();

    const result = await ModifyCashflowUseCase(repository).execute(command);

    expect(result.isSuccess).toBe(true);
    expect(repository.modifiedCashflows()).toEqual([command]);
  });

  it('propagates a repository failure', async () => {
    const repository = FlowsRepositoryInMemory();
    repository.isFailing(['boom']);

    const result = await ModifyCashflowUseCase(repository).execute(aModifyCashflowCommand());

    expect(result).toEqual(Result.failure(['boom']));
  });
});
