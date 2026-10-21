import { type AsyncResult, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { CategoryIncome } from '@/statistics/core/category-income';
import { WatchCategoryIncomeUseCase } from '@/statistics/core/watch-category-income/watch-category-income-use-case';

export const useCategoryIncome = (): AsyncResult<CategoryIncome[]> => {
  const { insightsRepository } = useRepositories();
  const settingsResult = useSettings();
  const [data, setData] = useState<AsyncResult<CategoryIncome[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);
  const effectiveDate = useMemo(() => today(), []);

  const useCase = useMemo(
    () => WatchCategoryIncomeUseCase(insightsRepository, effectiveDate, settings),
    [insightsRepository, effectiveDate, settings]
  );

  useEffect(() => {
    const unsubscribe = useCase.watch(setData);
    return () => unsubscribe();
  }, [useCase]);

  return data;
};
