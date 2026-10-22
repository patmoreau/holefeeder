import { type AsyncResult, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { PeriodTotals } from '@/summary/core/period-series';
import { WatchPeriodSeriesUseCase } from '@/summary/core/watch-period-series/watch-period-series-use-case';

export const usePeriodSeries = (count: number): AsyncResult<PeriodTotals[]> => {
  const { summaryRepository } = useRepositories();
  const settingsResult = useSettings();
  const [series, setSeries] = useState<AsyncResult<PeriodTotals[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);
  const asOf = useMemo(() => today(), []);

  const useCase = useMemo(() => WatchPeriodSeriesUseCase(settings, summaryRepository, asOf, count), [settings, summaryRepository, asOf, count]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setSeries);
    return () => unsubscribe();
  }, [useCase]);

  return series;
};
