import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';

export type CategoriesRepositoryInMemory = CategoriesRepository & {
  add: (...items: Category[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const CategoriesRepositoryInMemory = (): CategoriesRepositoryInMemory => {
  const itemsInMemory: Category[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const watch = (onDataChange: (result: AsyncResult<Category[]>) => void) => {
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else {
      onDataChange(Result.success(itemsInMemory));
    }
    // Return unsubscribe function
    return () => {};
  };

  const create = async () => Result.success(Id.newId());

  const update = async (command: { id: Id }) => Result.success(command.id);

  const add = (...items: Category[]) => itemsInMemory.push(...items);

  const isLoading = () => (loadingInMemory = true);

  const isFailing = (errors: string[]) => (errorsInMemory = errors);

  return { watch: watch, create: create, update: update, add: add, isLoading: isLoading, isFailing: isFailing };
};
