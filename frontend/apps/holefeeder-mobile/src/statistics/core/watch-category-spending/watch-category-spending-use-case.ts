import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { CategorySpending } from '../category-spending';
import { InsightsRepository } from '../insights-repository';

export const WatchCategorySpendingUseCase = (repository: InsightsRepository, effectiveDate: DateOnly, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<CategorySpending[]>) => void) =>
    repository.watchCategorySpending(onDataChange, effectiveDate, settings);

  return { watch: watch };
};
