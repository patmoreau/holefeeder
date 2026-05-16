import { useEffect, useMemo, useState } from 'react';
import { Transaction } from '@/flows/core/flows/transaction';
import { WatchFlowUseCase } from '@/flows/core/flows/watch-flow/watch-flow-use-case';
import { Id } from '@/shared/core/id';
import { type AsyncResult, Result } from '@/shared/core/result';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useFlow = (flowId: Id): AsyncResult<Transaction> => {
  const { flowRepository } = useRepositories();
  const [flow, setFlow] = useState<AsyncResult<Transaction>>(Result.loading());

  const useCase = useMemo(() => WatchFlowUseCase(flowId, flowRepository), [flowId, flowRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setFlow);
    return () => unsubscribe();
  }, [useCase]);

  return flow;
};
