import { type AsyncResult } from '@holefeeder/shared/core';
import { Settings } from '@/settings/core/settings';
import { InsightsRepository } from '../insights-repository';
import { TagSpending } from '../tag-spending';

export const WatchTagSpendingUseCase = (repository: InsightsRepository, settings: Settings) => {
  const watch = (onDataChange: (result: AsyncResult<TagSpending[]>) => void) => repository.watchTagSpending(onDataChange, settings);
  return { watch: watch };
};
