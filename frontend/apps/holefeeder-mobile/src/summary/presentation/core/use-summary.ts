import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { ComputedSummary, WatchSummaryUseCase } from '@/summary/core/watch-summary/watch-summary-use-case';

export const useSummary = (): AsyncResult<ComputedSummary> => {
  const { summaryRepository } = useRepositories();
  const settingsResult = useSettings();
  const [summary, setSummary] = useState<AsyncResult<ComputedSummary>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);

  const useCase = useMemo(() => {
    return WatchSummaryUseCase(settings, summaryRepository);
  }, [summaryRepository, settings]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setSummary);
    return () => unsubscribe();
  }, [useCase]);

  return summary;
};
