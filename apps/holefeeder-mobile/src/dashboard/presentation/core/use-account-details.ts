import { type AsyncResult, Result } from '@holefeeder/core';
import { useEffect, useMemo, useState } from 'react';
import { AccountSummary } from '@/accounts/core/account-summary';
import { WatchAccountDetailsUseCase } from '@/accounts/core/watch-account-details/watch-account-details-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useAccountDetails = (): AsyncResult<AccountSummary[]> => {
  const { accountRepository } = useRepositories();
  const [accounts, setAccounts] = useState<AsyncResult<AccountSummary[]>>(Result.loading());

  const useCase = useMemo(() => WatchAccountDetailsUseCase(accountRepository), [accountRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watchDetails(setAccounts);
    return () => unsubscribe();
  }, [useCase]);

  return accounts;
};
