import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';

export const DeactivateUpcomingFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (cashflowId: Id): Promise<Result<void>> => await repository.deactivateUpcoming(cashflowId);

  return {
    execute: execute,
  };
};
