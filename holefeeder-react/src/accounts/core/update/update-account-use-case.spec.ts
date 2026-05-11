import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { anUpdateAccountCommand } from '@/accounts/core/update/__tests__/update-account-command-for-test';
import { UpdateAccountUseCase } from '@/accounts/core/update/update-account-use-case';

describe('UpdateAccountUseCase', () => {
  it('returns success when account is updated', async () => {
    const account = anAccount();
    const repository = AccountsRepositoryInMemory();
    repository.add(account);

    const command = anUpdateAccountCommand({ id: account.id, name: 'Updated Name' });
    const useCase = UpdateAccountUseCase(repository);
    const result = await useCase.execute(command);

    expect(result).toBeSuccessWithValue(account.id);
  });

  it('returns failure when account does not exist', async () => {
    const repository = AccountsRepositoryInMemory();
    const command = anUpdateAccountCommand();
    const useCase = UpdateAccountUseCase(repository);

    const result = await useCase.execute(command);

    expect(result).toBeFailureWithErrors(['account-not-found']);
  });
});
