import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import type { Account } from '@/accounts/core/account';
import { WatchAccountsUseCase } from '@/accounts/core/watch-accounts/watch-accounts-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useAccounts = (): AsyncResult<Account[]> => {
  const { accountRepository } = useRepositories();
  const [accounts, setAccounts] = useState<AsyncResult<Account[]>>(Result.loading());

  const useCase = useMemo(() => WatchAccountsUseCase(accountRepository), [accountRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setAccounts);
    return () => unsubscribe();
  }, [useCase]);

  return accounts;
};
