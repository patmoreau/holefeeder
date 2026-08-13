import { DateOnly, DateOnlyErrors, Variation } from '@holefeeder/shared/core';
import { AccountErrors } from '@/accounts/core/account';
import { AccountType, AccountTypeErrors } from '@/accounts/core/account-type';
import { aCreateAccountCommand } from '@/accounts/core/create/__tests__/create-account-command-for-test';
import { CreateAccountCommand } from '@/accounts/core/create/create-account-command';

describe('CreateAccountCommand', () => {
  it('succeeds with valid data', () => {
    const cmd = aCreateAccountCommand();

    const result = CreateAccountCommand.create(cmd);

    expect(result).toBeSuccessWithValue({
      type: cmd.type,
      name: cmd.name,
      openBalance: Variation.valid(cmd.openBalance),
      openDate: DateOnly.valid(cmd.openDate),
      description: cmd.description,
      favorite: cmd.favorite,
    });
  });

  it('returns failure if name is empty', () => {
    const result = CreateAccountCommand.create(aCreateAccountCommand({ name: '' }));

    expect(result).toBeFailureWithErrors([AccountErrors.invalidName]);
  });

  it('returns failure if type is invalid', () => {
    const result = CreateAccountCommand.create(aCreateAccountCommand({ type: 'bad-type' as unknown as AccountType }));

    expect(result).toBeFailureWithErrors([AccountTypeErrors.invalid]);
  });

  it('returns failure if openDate is invalid', () => {
    const result = CreateAccountCommand.create(aCreateAccountCommand({ openDate: 'not-a-date' as DateOnly }));

    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });
});
