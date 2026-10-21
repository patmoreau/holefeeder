import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { InsightsRepository } from '../insights-repository';
import { TagIncome } from '../tag-income';

export const WatchTagIncomeUseCase = (repository: InsightsRepository, effectiveDate: DateOnly, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<TagIncome[]>) => void) => repository.watchTagIncome(onDataChange, effectiveDate, settings);
  return { watch: watch };
};
