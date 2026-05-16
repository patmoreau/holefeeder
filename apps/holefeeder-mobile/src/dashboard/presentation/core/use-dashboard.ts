import { useEffect, useMemo, useState } from 'react';
import { DashboardComputedSummary, WatchSummaryUseCase } from '@/dashboard/core/watch-summary/watch-summary-use-case';
import { DefaultSettings } from '@/settings/core/settings';
import { type AsyncResult, Result } from '@/shared/core/result';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useDashboard = (): AsyncResult<DashboardComputedSummary> => {
  const { dashboardRepository } = useRepositories();
  const settingsResult = useSettings();
  const [summary, setSummary] = useState<AsyncResult<DashboardComputedSummary>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);

  const useCase = useMemo(() => {
    return WatchSummaryUseCase(settings, dashboardRepository);
  }, [dashboardRepository, settings]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setSummary);
    return () => unsubscribe();
  }, [useCase]);

  return summary;
};
