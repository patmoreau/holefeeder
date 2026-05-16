import { Id, Result } from '@holefeeder/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';

export const DeactivateUpcomingFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (cashflowId: Id): Promise<Result<void>> => await repository.deactivateUpcoming(cashflowId);

  return {
    execute: execute,
  };
};
