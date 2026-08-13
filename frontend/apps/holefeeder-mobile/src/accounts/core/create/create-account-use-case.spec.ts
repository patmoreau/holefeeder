import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { aCreateAccountCommand } from '@/accounts/core/create/__tests__/create-account-command-for-test';
import { CreateAccountUseCase } from '@/accounts/core/create/create-account-use-case';

describe('CreateAccountUseCase', () => {
  let repository: AccountsRepositoryInMemory;

  beforeEach(() => {
    repository = AccountsRepositoryInMemory();
  });

  it('creates the account and answers with its new id', async () => {
    const command = aCreateAccountCommand();

    const result = await CreateAccountUseCase(repository).execute(command);

    expect(result.isSuccess).toBe(true);
    expect(repository.createdCommands()).toEqual([command]);
  });

  it('propagates a repository failure', async () => {
    repository.isFailing(['boom']);

    const result = await CreateAccountUseCase(repository).execute(aCreateAccountCommand());

    expect(result).toBeFailureWithErrors(['boom']);
  });
});
