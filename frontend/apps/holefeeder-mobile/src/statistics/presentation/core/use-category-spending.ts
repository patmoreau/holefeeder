import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/settings/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { CategorySpending } from '@/statistics/core/category-spending';
import { WatchCategorySpendingUseCase } from '@/statistics/core/watch-category-spending/watch-category-spending-use-case';

export const useCategorySpending = (): AsyncResult<CategorySpending[]> => {
  const { insightsRepository } = useRepositories();
  const settingsResult = useSettings();
  const [data, setData] = useState<AsyncResult<CategorySpending[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);

  const useCase = useMemo(() => WatchCategorySpendingUseCase(insightsRepository, settings), [insightsRepository, settings]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setData);
    return () => unsubscribe();
  }, [useCase]);

  return data;
};
