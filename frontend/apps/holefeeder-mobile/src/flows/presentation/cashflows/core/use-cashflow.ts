import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { FlowsRepositoryErrors } from '@/flows/core/flows/flows-repository';
import { WatchCashflowsUseCase } from '@/flows/core/flows/watch-cashflows/watch-cashflows-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useCashflow = (id: Id): AsyncResult<Cashflow> => {
  const { flowRepository } = useRepositories();
  const [cashflow, setCashflow] = useState<AsyncResult<Cashflow>>(Result.loading());

  const useCase = useMemo(() => WatchCashflowsUseCase(flowRepository), [flowRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch((result) => {
      if (result.isLoading) {
        setCashflow(Result.loading());
      } else if (result.isFailure) {
        setCashflow(Result.failure(result.errors));
      } else {
        const found = result.value.find((c) => c.id === id);
        setCashflow(found ? Result.success(found) : Result.failure([FlowsRepositoryErrors.modifyCashflowCommandFailed]));
      }
    });
    return () => unsubscribe();
  }, [useCase, id]);

  return cashflow;
};
