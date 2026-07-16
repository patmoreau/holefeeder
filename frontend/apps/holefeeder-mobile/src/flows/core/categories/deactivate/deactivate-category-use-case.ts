import { Id, Result } from '@holefeeder/shared/core';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';

export const DeactivateCategoryUseCase = (repository: CategoriesRepository) => {
  const execute = async (id: Id): Promise<Result<void>> => await repository.deactivate(id);

  return { execute };
};
