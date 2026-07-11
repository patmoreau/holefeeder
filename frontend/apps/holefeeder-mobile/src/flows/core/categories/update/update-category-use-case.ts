import { Id, Result } from '@holefeeder/shared/core';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';

export const UpdateCategoryUseCase = (repository: CategoriesRepository) => {
  const execute = async (command: UpdateCategoryCommand): Promise<Result<Id>> => await repository.update(command);

  return { execute };
};
