import { type AsyncResult, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { DefaultSettings } from '@/shared/core/settings';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { TagSpending } from '@/statistics/core/tag-spending';
import { WatchTagSpendingUseCase } from '@/statistics/core/watch-tag-spending/watch-tag-spending-use-case';

export const useTagSpending = (): AsyncResult<TagSpending[]> => {
  const { insightsRepository } = useRepositories();
  const settingsResult = useSettings();
  const [data, setData] = useState<AsyncResult<TagSpending[]>>(Result.loading());

  const settings = useMemo(() => (settingsResult.isSuccess ? settingsResult.value : DefaultSettings), [settingsResult]);
  const effectiveDate = useMemo(() => today(), []);

  const useCase = useMemo(
    () => WatchTagSpendingUseCase(insightsRepository, effectiveDate, settings),
    [insightsRepository, effectiveDate, settings]
  );

  useEffect(() => {
    const unsubscribe = useCase.watch(setData);
    return () => unsubscribe();
  }, [useCase]);

  return data;
};
