import { Id, Result } from '@holefeeder/core';
import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';

export const UpdateAccountUseCase = (repository: AccountsRepository) => {
  const execute = async (command: UpdateAccountCommand): Promise<Result<Id>> => await repository.update(command);

  return { execute };
};
