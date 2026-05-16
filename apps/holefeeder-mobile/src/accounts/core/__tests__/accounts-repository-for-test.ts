import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { Account } from '@/accounts/core/account';
import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';

export type AccountsRepositoryInMemory = AccountsRepository & {
  add: (...items: Account[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const AccountsRepositoryInMemory = (): AccountsRepositoryInMemory => {
  const itemsInMemory: Account[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const watch = (onDataChange: (result: AsyncResult<Account[]>) => void) => {
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else {
      onDataChange(Result.success(itemsInMemory));
    }
    return () => {};
  };

  const update = async (command: UpdateAccountCommand): Promise<Result<Id>> => {
    const index = itemsInMemory.findIndex((a) => a.id === command.id);
    if (index === -1) return Result.failure(['account-not-found']);
    itemsInMemory[index] = Account.valid({ ...command });
    return Result.success(command.id);
  };

  const add = (...items: Account[]) => itemsInMemory.push(...items);

  const isLoading = () => (loadingInMemory = true);

  const isFailing = (errors: string[]) => (errorsInMemory = errors);

  return { watch, update, add, isLoading, isFailing };
};
