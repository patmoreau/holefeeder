import { type AsyncResult, Id, Result } from '@holefeeder/shared/core';
import { Account } from '@/accounts/core/account';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';

export type AccountsRepository = {
  watch: (onDataChange: (result: AsyncResult<Account[]>) => void) => () => void;
  update: (command: UpdateAccountCommand) => Promise<Result<Id>>;
};

export const AccountsRepositoryErrors = {
  noAccounts: 'no-accounts',
  accountNotFound: 'account-not-found',
};
