import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { CategoriesRepositoryErrors } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';
import { WatchCategoriesUseCase } from '@/flows/core/categories/watch-categories/watch-categories-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useCategory = (id: Id): AsyncResult<Category> => {
  const { categoryRepository } = useRepositories();
  const [category, setCategory] = useState<AsyncResult<Category>>(Result.loading());

  const useCase = useMemo(() => WatchCategoriesUseCase(categoryRepository), [categoryRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch((result) => {
      if (result.isLoading) {
        setCategory(Result.loading());
      } else if (result.isFailure) {
        setCategory(Result.failure(result.errors));
      } else {
        const found = result.value.find((c) => c.id === id);
        setCategory(found ? Result.success(found) : Result.failure([CategoriesRepositoryErrors.categoryNotFound]));
      }
    });
    return () => unsubscribe();
  }, [useCase, id]);

  return category;
};
