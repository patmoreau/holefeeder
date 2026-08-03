import { type AsyncResult } from '@holefeeder/shared/core';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';

export const WatchCashflowsUseCase = (repository: FlowsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Cashflow[]>) => void) => repository.watchCashflows(onDataChange);

  return {
    watch: watch,
  };
};
