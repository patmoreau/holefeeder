import { type AsyncResult } from '@holefeeder/shared/core';
import { Settings } from '@/settings/core/settings';
import { CategorySpending } from '../category-spending';
import { InsightsRepository } from '../insights-repository';

export const WatchCategorySpendingUseCase = (repository: InsightsRepository, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<CategorySpending[]>) => void) => repository.watchCategorySpending(onDataChange, settings);

  return { watch: watch };
};
