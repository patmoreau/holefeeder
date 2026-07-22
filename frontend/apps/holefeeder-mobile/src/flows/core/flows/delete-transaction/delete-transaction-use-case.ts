import { Id, Result } from '@holefeeder/shared/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';

export const DeleteTransactionUseCase = (repository: FlowsRepository) => {
  const execute = async (transactionId: Id): Promise<Result<void>> => await repository.deleteTransaction(transactionId);

  return {
    execute: execute,
  };
};
