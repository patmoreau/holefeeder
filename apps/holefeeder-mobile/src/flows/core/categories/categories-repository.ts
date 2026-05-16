import type { AsyncResult } from '@holefeeder/core';
import { Category } from './category';

export type CategoriesRepository = {
  watch: (onDataChange: (result: AsyncResult<Category[]>) => void) => () => void;
};

export const CategoriesRepositoryErrors = {
  noCategories: 'no-categories',
};
