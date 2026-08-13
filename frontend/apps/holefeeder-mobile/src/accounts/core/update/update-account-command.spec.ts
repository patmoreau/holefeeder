import {
  DateOnly,
  DateOnlyErrors,
  Favorite,
  FavoriteErrors,
  Id,
  IdErrors,
  Inactive,
  InactiveErrors,
  Variation,
  VariationErrors,
} from '@holefeeder/shared/core';
import { AccountErrors } from '@/accounts/core/account';
import { AccountType, AccountTypeErrors } from '@/accounts/core/account-type';
import { anUpdateAccountCommand } from '@/accounts/core/update/__tests__/update-account-command-for-test';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';

describe('UpdateAccountCommand', () => {
  it('succeeds with valid data', () => {
    const cmd = anUpdateAccountCommand();
    const result = UpdateAccountCommand.create(cmd);
    expect(result).toBeSuccessWithValue({
      id: Id.valid(cmd.id),
      type: cmd.type,
      name: cmd.name,
      openBalance: Variation.valid(cmd.openBalance),
      openDate: DateOnly.valid(cmd.openDate),
      description: cmd.description,
      favorite: cmd.favorite,
      inactive: cmd.inactive,
    });
  });

  it('returns failure if id is invalid', () => {
    const result = UpdateAccountCommand.create(anUpdateAccountCommand({ id: '' as Id }));
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if name is empty', () => {
    const result = UpdateAccountCommand.create(anUpdateAccountCommand({ name: '' }));
    expect(result).toBeFailureWithErrors([AccountErrors.invalidName]);
  });

  it('returns failure if type is invalid', () => {
    const result = UpdateAccountCommand.create(anUpdateAccountCommand({ type: 'bad-type' as unknown as AccountType }));
    expect(result).toBeFailureWithErrors([AccountTypeErrors.invalid]);
  });

  it('returns failure if openDate is invalid', () => {
    const result = UpdateAccountCommand.create(anUpdateAccountCommand({ openDate: 'not-a-date' as DateOnly }));
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if openBalance is invalid', () => {
    const result = UpdateAccountCommand.create(anUpdateAccountCommand({ openBalance: NaN as Variation }));
    expect(result).toBeFailureWithErrors([VariationErrors.invalid]);
  });

  it('returns failure if favorite is invalid', () => {
    const result = UpdateAccountCommand.create(
      anUpdateAccountCommand({
        favorite: 'not-a-boolean' as unknown as Favorite,
      })
    );
    expect(result).toBeFailureWithErrors([FavoriteErrors.invalid]);
  });

  it('returns failure if inactive is invalid', () => {
    const result = UpdateAccountCommand.create(
      anUpdateAccountCommand({
        inactive: 'not-a-boolean' as unknown as Inactive,
      })
    );
    expect(result).toBeFailureWithErrors([InactiveErrors.invalid]);
  });

  it('accepts empty description', () => {
    const cmd = anUpdateAccountCommand({ description: '' });
    const result = UpdateAccountCommand.create(cmd);
    expect(result).toBeSuccessWithValue(expect.objectContaining({ description: '' }));
  });
});
