import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { CategoryIncome } from '@/statistics/core/category-income';
import { CategorySpending } from '@/statistics/core/category-spending';
import { CategoryTagSpending } from '@/statistics/core/category-tag-spending';
import { InsightsRepository } from '@/statistics/core/insights-repository';
import { TagIncome } from '@/statistics/core/tag-income';
import { TagSpending } from '@/statistics/core/tag-spending';

export type InsightsRepositoryInMemory = InsightsRepository & {
  addCategorySpending: (...items: CategorySpending[]) => void;
  addCategoryIncome: (...items: CategoryIncome[]) => void;
  addTagSpending: (...items: TagSpending[]) => void;
  addTagIncome: (...items: TagIncome[]) => void;
  addCategoryTagSpending: (...items: CategoryTagSpending[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const InsightsRepositoryInMemory = (): InsightsRepositoryInMemory => {
  const categorySpendingInMemory: CategorySpending[] = [];
  const categoryIncomeInMemory: CategoryIncome[] = [];
  const tagSpendingInMemory: TagSpending[] = [];
  const tagIncomeInMemory: TagIncome[] = [];
  const categoryTagSpendingInMemory: CategoryTagSpending[] = [];
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
    watchCategoryIncome: (onDataChange) => emit(categoryIncomeInMemory, onDataChange),
    watchTagSpending: (onDataChange) => emit(tagSpendingInMemory, onDataChange),
    watchTagIncome: (onDataChange) => emit(tagIncomeInMemory, onDataChange),
    watchCategoryTagSpending: (onDataChange) => emit(categoryTagSpendingInMemory, onDataChange),
    addCategorySpending: (...items) => categorySpendingInMemory.push(...items),
    addCategoryIncome: (...items) => categoryIncomeInMemory.push(...items),
    addTagSpending: (...items) => tagSpendingInMemory.push(...items),
    addTagIncome: (...items) => tagIncomeInMemory.push(...items),
    addCategoryTagSpending: (...items) => categoryTagSpendingInMemory.push(...items),
    isLoading: () => (loadingInMemory = true),
    isFailing: (errors) => (errorsInMemory = errors),
  };
};
