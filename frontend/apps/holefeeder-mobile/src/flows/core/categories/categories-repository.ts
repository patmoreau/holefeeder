import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { Category } from './category';
import { CreateCategoryCommand } from './create/create-category-command';
import { UpdateCategoryCommand } from './update/update-category-command';

export type CategoriesRepository = {
  watch: (onDataChange: (result: AsyncResult<Category[]>) => void) => () => void;
  create: (command: CreateCategoryCommand) => Promise<Result<Id>>;
  update: (command: UpdateCategoryCommand) => Promise<Result<Id>>;
  deactivate: (id: Id) => Promise<Result<void>>;
};

export const CategoriesRepositoryErrors = {
  noCategories: 'no-categories',
  categoryNotFound: 'category-not-found',
};
