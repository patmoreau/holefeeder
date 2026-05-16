import { type AsyncResult } from '@/shared/core/result';
import { CategoriesRepository } from '../categories-repository';
import { Category } from '../category';

export const WatchCategoriesUseCase = (repository: CategoriesRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Category[]>) => void) =>
    repository.watch((result: AsyncResult<Category[]>) => {
      if (result.isLoading || result.isFailure) {
        onDataChange(result);
        return;
      }

      onDataChange(result);
    });

  return {
    watch: watch,
  };
};
