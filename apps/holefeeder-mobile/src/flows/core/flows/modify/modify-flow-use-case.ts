import { Id, Result } from '@holefeeder/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';

export const ModifyFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: ModifyFlowCommand): Promise<Result<Id>> => await repository.modify(flow);

  return {
    execute: execute,
  };
};
