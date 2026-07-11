import { Id, Result } from '@holefeeder/shared/core';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';

export const CreateCategoryUseCase = (repository: CategoriesRepository) => {
  const execute = async (command: CreateCategoryCommand): Promise<Result<Id>> => await repository.create(command);

  return { execute };
};
