import { type AsyncResult, Money, Result } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { CombinedCategorySpending } from '@/statistics/core/combined-category-spending';
import { useCategorySpending } from './use-category-spending';
import { useCategoryTagSpending } from './use-category-tag-spending';

export const useCombinedSpending = (): AsyncResult<CombinedCategorySpending[]> => {
  const categoriesResult = useCategorySpending();
  const tagsResult = useCategoryTagSpending();

  return useMemo(() => {
    const combined = Result.combine({ categories: categoriesResult, tags: tagsResult });
    if (!combined.isSuccess) return combined;

    const { categories, tags } = combined.value;

    return Result.success(
      categories.map((category) => ({
        category,
        tags: tags
          .filter((tag) => tag.categoryId === category.categoryId)
          .sort((a, b) => Money.toCents(b.spentAmount) - Money.toCents(a.spentAmount)),
      }))
    );
  }, [categoriesResult, tagsResult]);
};
