import { Id, Result } from '@holefeeder/shared/core';
import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { CreateAccountCommand } from '@/accounts/core/create/create-account-command';

export const CreateAccountUseCase = (repository: AccountsRepository) => {
  const execute = async (command: CreateAccountCommand): Promise<Result<Id>> => await repository.create(command);

  return { execute };
};
