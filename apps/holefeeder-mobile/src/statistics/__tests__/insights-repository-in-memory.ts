import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { CategorySpending } from '@/statistics/core/category-spending';
import { InsightsRepository } from '@/statistics/core/insights-repository';
import { TagSpending } from '@/statistics/core/tag-spending';

export type InsightsRepositoryInMemory = InsightsRepository & {
  addCategorySpending: (...items: CategorySpending[]) => void;
  addTagSpending: (...items: TagSpending[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const InsightsRepositoryInMemory = (): InsightsRepositoryInMemory => {
  const categorySpendingInMemory: CategorySpending[] = [];
  const tagSpendingInMemory: TagSpending[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const emit = <T>(items: T[], onDataChange: (result: AsyncResult<T[]>) => void) => {
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else {
      onDataChange(Result.success(items));
    }
    return () => {};
  };

  return {
    watchCategorySpending: (onDataChange) => emit(categorySpendingInMemory, onDataChange),
    watchTagSpending: (onDataChange) => emit(tagSpendingInMemory, onDataChange),
    addCategorySpending: (...items) => categorySpendingInMemory.push(...items),
    addTagSpending: (...items) => tagSpendingInMemory.push(...items),
    isLoading: () => (loadingInMemory = true),
    isFailing: (errors) => (errorsInMemory = errors),
  };
};
