import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { CategorySpending } from './category-spending';
import { CategoryTagSpending } from './category-tag-spending';
import { TagSpending } from './tag-spending';

export type InsightsRepository = {
  watchCategorySpending: (
    onDataChange: (result: AsyncResult<CategorySpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
  watchTagSpending: (onDataChange: (result: AsyncResult<TagSpending[]>) => void, effectiveDate: DateOnly, settings: Settings) => () => void;
  watchCategoryTagSpending: (
    onDataChange: (result: AsyncResult<CategoryTagSpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
};
