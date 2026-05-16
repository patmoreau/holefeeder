import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';
import { CreateFlowCommand } from './create-flow-command';

export const CreateFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: CreateFlowCommand): Promise<Result<Id>> => await repository.create(flow);

  return {
    execute: execute,
  };
};
