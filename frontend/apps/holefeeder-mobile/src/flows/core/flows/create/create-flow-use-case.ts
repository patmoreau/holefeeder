import { Id, Result } from '@holefeeder/shared/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { CreateFlowCommand } from './create-flow-command';

export const CreateFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: CreateFlowCommand): Promise<Result<Id>> => await repository.create(flow);

  return {
    execute: execute,
  };
};
