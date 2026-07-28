import { type AsyncResult, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { CategoryTagSpending } from '@/statistics/core/category-tag-spending';
import { WatchCategoryTagSpendingUseCase } from '@/statistics/core/watch-category-tag-spending/watch-category-tag-spending-use-case';

export const useCategoryTagSpending = (): AsyncResult<CategoryTagSpending[]> => {
  const { insightsRepository } = useRepositories();
  const settingsResult = useSettings();
  const [data, setData] = useState<AsyncResult<CategoryTagSpending[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);
  const effectiveDate = useMemo(() => today(), []);

  const useCase = useMemo(
    () => WatchCategoryTagSpendingUseCase(insightsRepository, effectiveDate, settings),
    [insightsRepository, effectiveDate, settings]
  );

  useEffect(() => {
    const unsubscribe = useCase.watch(setData);
    return () => unsubscribe();
  }, [useCase]);

  return data;
};
