import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { WatchCashflowsUseCase } from '@/flows/core/flows/watch-cashflows/watch-cashflows-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useCashflows = (): AsyncResult<Cashflow[]> => {
  const { flowRepository } = useRepositories();
  const [cashflows, setCashflows] = useState<AsyncResult<Cashflow[]>>(Result.loading());

  const useCase = useMemo(() => WatchCashflowsUseCase(flowRepository), [flowRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setCashflows);
    return () => unsubscribe();
  }, [useCase]);

  return cashflows;
};
