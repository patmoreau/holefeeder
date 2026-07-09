import { type AsyncResult, Id } from '@holefeeder/shared/core';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { Transaction } from '@/flows/core/flows/transaction';

export const WatchFlowUseCase = (id: Id, flowsRepository: FlowsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Transaction>) => void) =>
    flowsRepository.watchTransaction(id, (result) => {
      onDataChange(result);
    });

  return { watch: watch };
};
