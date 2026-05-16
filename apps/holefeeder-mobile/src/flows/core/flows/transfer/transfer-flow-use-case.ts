import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { Result } from '@/shared/core/result';

export const TransferFlowUseCase = (repository: FlowsRepository) => {
  const execute = async (flow: TransferFlowCommand): Promise<Result<void>> => await repository.transfer(flow);

  return {
    execute: execute,
  };
};
