import { Id, Result } from '@holefeeder/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { PayFlowCommand } from './pay-flow-command';

export const PayUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: PayFlowCommand): Promise<Result<Id>> => await repository.pay(flow);

  return {
    execute: execute,
  };
};
