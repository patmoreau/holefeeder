import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/settings/core/settings';
import { CategorySpending } from './category-spending';
import { TagSpending } from './tag-spending';

export type InsightsRepository = {
  watchCategorySpending: (
    onDataChange: (result: AsyncResult<CategorySpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
  watchTagSpending: (onDataChange: (result: AsyncResult<TagSpending[]>) => void, effectiveDate: DateOnly, settings: Settings) => () => void;
};
