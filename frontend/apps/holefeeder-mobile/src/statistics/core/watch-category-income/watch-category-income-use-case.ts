import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { CategoryIncome } from '../category-income';
import { InsightsRepository } from '../insights-repository';

export const WatchCategoryIncomeUseCase = (repository: InsightsRepository, effectiveDate: DateOnly, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<CategoryIncome[]>) => void) =>
    repository.watchCategoryIncome(onDataChange, effectiveDate, settings);

  return { watch: watch };
};
