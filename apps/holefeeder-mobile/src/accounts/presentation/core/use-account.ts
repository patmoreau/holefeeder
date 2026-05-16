import { type AsyncResult, Id, Result } from '@holefeeder/core';
import { useEffect, useMemo, useState } from 'react';
import { Account } from '@/accounts/core/account';
import { WatchAccountsUseCase } from '@/accounts/core/watch-accounts/watch-accounts-use-case';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useAccount = (id: Id): AsyncResult<Account> => {
  const { accountRepository } = useRepositories();
  const [account, setAccount] = useState<AsyncResult<Account>>(Result.loading());

  const useCase = useMemo(() => WatchAccountsUseCase(accountRepository), [accountRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch((result) => {
      if (result.isLoading) {
        setAccount(Result.loading());
      } else if (result.isFailure) {
        setAccount(Result.failure(result.errors));
      } else {
        const found = result.value.find((a) => a.id === id);
        setAccount(found ? Result.success(found) : Result.failure(['account-not-found']));
      }
    });
    return () => unsubscribe();
  }, [useCase, id]);

  return account;
};
