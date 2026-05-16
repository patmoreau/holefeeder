import { useEffect, useMemo, useState } from 'react';
import type { Category } from '@/flows/core/categories/category';
import { WatchCategoriesUseCase } from '@/flows/core/categories/watch-categories/watch-categories-use-case';
import { type AsyncResult, Result } from '@/shared/core/result';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useCategories = (): AsyncResult<Category[]> => {
  const { categoryRepository } = useRepositories();
  const [categories, setCategories] = useState<AsyncResult<Category[]>>(Result.loading());

  const useCase = useMemo(() => WatchCategoriesUseCase(categoryRepository), [categoryRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setCategories);
    return () => unsubscribe();
  }, [useCase]);

  return categories;
};
