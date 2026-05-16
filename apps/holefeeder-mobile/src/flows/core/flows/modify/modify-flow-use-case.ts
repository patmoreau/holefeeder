import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';

export const ModifyFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: ModifyFlowCommand): Promise<Result<Id>> => await repository.modify(flow);

  return {
    execute: execute,
  };
};
