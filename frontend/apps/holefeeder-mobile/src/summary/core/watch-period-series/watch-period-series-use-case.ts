import { type AsyncResult, DateOnly, Result } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { PeriodSeries, PeriodTotals } from '../period-series';
import { SummaryData } from '../summary-data';
import { SummaryRepository } from '../summary-repository';

export const WatchPeriodSeriesUseCase = (settings: Settings, repository: SummaryRepository, asOf: DateOnly, count: number) => {
  const watch = (onDataChange: (result: AsyncResult<PeriodTotals[]>) => void) =>
    repository.watch((result: AsyncResult<SummaryData[]>) => {
      if (result.isLoading || result.isFailure) {
        onDataChange(result);
        return;
      }

      onDataChange(Result.success(PeriodSeries.of(result.value, settings, asOf, count)));
    }, settings);

  return {
    watch: watch,
  };
};
