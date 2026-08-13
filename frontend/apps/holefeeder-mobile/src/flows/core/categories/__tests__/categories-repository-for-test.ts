import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';

export type CategoriesRepositoryInMemory = CategoriesRepository & {
  add: (...items: Category[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
  deactivatedIds: () => Id[];
  createdCommands: () => CreateCategoryCommand[];
};

export const CategoriesRepositoryInMemory = (): CategoriesRepositoryInMemory => {
  const itemsInMemory: Category[] = [];
  const deactivatedInMemory: Id[] = [];
  const createdInMemory: CreateCategoryCommand[] = [];
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

  const create = async (command: CreateCategoryCommand) => {
    createdInMemory.push(command);
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    return Result.success(Id.newId());
  };

  const update = async (command: { id: Id }) => Result.success(command.id);

  const deactivate = async (id: Id) => {
    if (errorsInMemory.length > 0) {
      return Result.failure(errorsInMemory);
    }
    deactivatedInMemory.push(id);
    return Result.success();
  };

  const add = (...items: Category[]) => itemsInMemory.push(...items);

  const isLoading = () => (loadingInMemory = true);

  const isFailing = (errors: string[]) => (errorsInMemory = errors);

  const deactivatedIds = () => deactivatedInMemory;

  const createdCommands = () => createdInMemory;

  return {
    watch: watch,
    create: create,
    update: update,
    deactivate: deactivate,
    add: add,
    isLoading: isLoading,
    isFailing: isFailing,
    deactivatedIds: deactivatedIds,
    createdCommands: createdCommands,
  };
};
