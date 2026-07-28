import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { InsightsRepository } from '../insights-repository';
import { TagSpending } from '../tag-spending';

export const WatchTagSpendingUseCase = (repository: InsightsRepository, effectiveDate: DateOnly, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<TagSpending[]>) => void) =>
    repository.watchTagSpending(onDataChange, effectiveDate, settings);
  return { watch: watch };
};
