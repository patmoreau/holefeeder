import { type AsyncResult } from '@/shared/core/result';
import { Account } from '../account';
import { AccountsRepository } from '../accounts-repository';

export const WatchAccountsUseCase = (accountsRepository: AccountsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<Account[]>) => void) => accountsRepository.watch((result) => onDataChange(result));

  return {
    watch: watch,
  };
};
