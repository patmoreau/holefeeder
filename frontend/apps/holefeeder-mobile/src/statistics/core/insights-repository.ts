import { type AsyncResult } from '@holefeeder/shared/core';
import { Settings } from '@/settings/core/settings';
import { CategorySpending } from './category-spending';
import { TagSpending } from './tag-spending';

export type InsightsRepository = {
  watchCategorySpending: (onDataChange: (result: AsyncResult<CategorySpending[]>) => void, settings: Settings) => () => void;
  watchTagSpending: (onDataChange: (result: AsyncResult<TagSpending[]>) => void, settings: Settings) => () => void;
};
