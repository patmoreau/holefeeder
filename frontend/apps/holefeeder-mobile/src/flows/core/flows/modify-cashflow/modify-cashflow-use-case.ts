import { Id, Result } from '@holefeeder/shared/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { ModifyCashflowCommand } from '@/flows/core/flows/modify-cashflow/modify-cashflow-command';

export const ModifyCashflowUseCase = (repository: FlowsRepository) => {
  const execute = async (cashflow: ModifyCashflowCommand): Promise<Result<Id>> => await repository.modifyCashflow(cashflow);

  return {
    execute: execute,
  };
};
