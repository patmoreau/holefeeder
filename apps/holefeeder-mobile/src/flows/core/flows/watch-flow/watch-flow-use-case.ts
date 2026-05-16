import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { Transaction } from '@/flows/core/flows/transaction';
import { Id } from '@/shared/core/id';
import { type AsyncResult } from '@/shared/core/result';

export const WatchFlowUseCase = (id: Id, flowsRepository: FlowsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Transaction>) => void) =>
    flowsRepository.watchTransaction(id, (result) => {
      onDataChange(result);
    });

  return { watch: watch };
};
