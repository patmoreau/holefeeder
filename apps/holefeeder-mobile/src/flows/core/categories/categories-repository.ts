import { type AsyncResult } from '@/shared/core/result';
import { Category } from './category';

export type CategoriesRepository = {
  watch: (onDataChange: (result: AsyncResult<Category[]>) => void) => () => void;
};

export const CategoriesRepositoryErrors = {
  noCategories: 'no-categories',
};
