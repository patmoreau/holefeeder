import { AccountsRepository } from '@/accounts/core/accounts-repository';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';
import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';

export const UpdateAccountUseCase = (repository: AccountsRepository) => {
  const execute = async (command: UpdateAccountCommand): Promise<Result<Id>> => await repository.update(command);

  return { execute };
};
