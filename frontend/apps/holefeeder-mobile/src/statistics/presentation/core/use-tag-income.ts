import { type AsyncResult, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { TagIncome } from '@/statistics/core/tag-income';
import { WatchTagIncomeUseCase } from '@/statistics/core/watch-tag-income/watch-tag-income-use-case';

export const useTagIncome = (): AsyncResult<TagIncome[]> => {
  const { insightsRepository } = useRepositories();
  const settingsResult = useSettings();
  const [data, setData] = useState<AsyncResult<TagIncome[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);
  const effectiveDate = useMemo(() => today(), []);

  const useCase = useMemo(
    () => WatchTagIncomeUseCase(insightsRepository, effectiveDate, settings),
    [insightsRepository, effectiveDate, settings]
  );

  useEffect(() => {
    const unsubscribe = useCase.watch(setData);
    return () => unsubscribe();
  }, [useCase]);

  return data;
};
