import { useEffect, useMemo, useState } from 'react';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { WatchUpcomingFlowsUseCase } from '@/flows/core/flows/watch-upcoming/watch-upcoming-flows-use-case';
import { DefaultSettings } from '@/settings/core/settings';
import { type AsyncResult, Result } from '@/shared/core/result';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useUpcomingFlows = (): AsyncResult<UpcomingFlow[]> => {
  const { flowRepository } = useRepositories();
  const settingsResult = useSettings();
  const [upcomingFlows, setUpcomingFlows] = useState<AsyncResult<UpcomingFlow[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);

  const useCase = useMemo(() => {
    if (settingsResult.isLoading) return null;

    return WatchUpcomingFlowsUseCase(settings, flowRepository);
  }, [flowRepository, settings, settingsResult.isLoading]);

  useEffect(() => {
    if (!useCase) {
      return;
    }

    const unsubscribe = useCase.watch(setUpcomingFlows);
    return () => unsubscribe();
  }, [useCase]);

  return upcomingFlows;
};
