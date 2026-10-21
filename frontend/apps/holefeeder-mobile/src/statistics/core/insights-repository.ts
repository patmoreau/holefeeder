import { type AsyncResult, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { CategoryIncome } from './category-income';
import { CategorySpending } from './category-spending';
import { CategoryTagSpending } from './category-tag-spending';
import { TagIncome } from './tag-income';
import { TagSpending } from './tag-spending';

export type InsightsRepository = {
  watchCategorySpending: (
    onDataChange: (result: AsyncResult<CategorySpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
  watchCategoryIncome: (
    onDataChange: (result: AsyncResult<CategoryIncome[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
  watchTagSpending: (onDataChange: (result: AsyncResult<TagSpending[]>) => void, effectiveDate: DateOnly, settings: Settings) => () => void;
  watchTagIncome: (onDataChange: (result: AsyncResult<TagIncome[]>) => void, effectiveDate: DateOnly, settings: Settings) => () => void;
  watchCategoryTagSpending: (
    onDataChange: (result: AsyncResult<CategoryTagSpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => () => void;
};
