import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { CategoryTagSpending } from '../category-tag-spending';
import { InsightsRepository } from '../insights-repository';

export const WatchCategoryTagSpendingUseCase = (repository: InsightsRepository, effectiveDate: DateOnly, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<CategoryTagSpending[]>) => void) =>
    repository.watchCategoryTagSpending(onDataChange, effectiveDate, settings);

  return { watch: watch };
};
