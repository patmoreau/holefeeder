import { useEffect, useMemo, useState } from 'react';
import { AccountDetail } from '@/accounts/core/account-detail';
import { WatchAccountVariationUseCase } from '@/accounts/core/watch-account-variation/watch-account-variation-use-case';
import { DefaultSettings } from '@/settings/core/settings';
import { DateInterval } from '@/shared/core/date-interval';
import { Id } from '@/shared/core/id';
import { type AsyncResult, Result } from '@/shared/core/result';
import { today } from '@/shared/core/with-date';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useAccountVariation = (id: Id): AsyncResult<AccountDetail> => {
  const { accountRepository, flowRepository } = useRepositories();
  const settingsResult = useSettings();
  const [account, setAccount] = useState<AsyncResult<AccountDetail>>(Result.loading());

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;

  const dateIntervalResult = useMemo(
    () => DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency),
    [settings.effectiveDate, settings.intervalType, settings.frequency]
  );

  const useCase = useMemo(() => {
    if (!dateIntervalResult.isSuccess) {
      return null;
    }
    return WatchAccountVariationUseCase(id, dateIntervalResult.value, accountRepository, flowRepository);
  }, [id, dateIntervalResult, accountRepository, flowRepository]);

  useEffect(() => {
    if (!dateIntervalResult.isSuccess) {
      setAccount(Result.failure(dateIntervalResult.isFailure ? dateIntervalResult.errors : []));
      return;
    }
    if (!useCase) {
      return;
    }
    const unsubscribe = useCase.watchVariation(setAccount);
    return () => unsubscribe();
  }, [useCase, dateIntervalResult]);

  return account;
};
