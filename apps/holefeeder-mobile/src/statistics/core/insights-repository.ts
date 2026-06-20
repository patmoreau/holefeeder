import { type AsyncResult } from '@holefeeder/shared/core';
import { Settings } from '@/settings/core/settings';
import { CategorySpending } from './category-spending';

export type InsightsRepository = {
  watchCategorySpending: (onDataChange: (result: AsyncResult<CategorySpending[]>) => void, settings: Settings) => () => void;
};
