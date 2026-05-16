import { anAccount, toAccount } from '@/accounts/core/__tests__/account-for-test';
import { Account, AccountErrors } from '@/accounts/core/account';
import { DateOnlyErrors } from '@/shared/core/date-only';
import { IdErrors } from '@/shared/core/id';
import { VariationErrors } from '@/shared/core/variation';

describe('Account', () => {
  const validAccount = anAccount();

  it('create a valid account', () => {
    const result = Account.create(validAccount);

    expect(result).toBeSuccessWithValue(toAccount(validAccount));
  });

  it('rejects invalid name (empty)', () => {
    const result = Account.create({ ...validAccount, name: '' });
    expect(result).toBeFailureWithErrors([AccountErrors.invalidName]);
  });

  it('rejects invalid name (wrong type)', () => {
    const result = Account.create({ ...validAccount, name: 123 });
    expect(result).toBeFailureWithErrors([AccountErrors.invalidName]);
  });

  it('rejects invalid openBalance', () => {
    const result = Account.create({ ...validAccount, openBalance: NaN });
    expect(result).toBeFailureWithErrors([VariationErrors.invalid]);
  });

  it('rejects invalid openDate', () => {
    const result = Account.create({ ...validAccount, openDate: 'invalid-date' });
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('rejects invalid description (wrong type)', () => {
    const result = Account.create({ ...validAccount, description: 123 });
    expect(result).toBeFailureWithErrors([AccountErrors.invalidDescription]);
  });

  it('rejects invalid favorite (wrong type)', () => {
    const result = Account.create({ ...validAccount, favorite: 'yes' });
    expect(result).toBeFailureWithErrors([AccountErrors.invalidFavorite]);
  });

  it('rejects invalid inactive (wrong type)', () => {
    const result = Account.create({ ...validAccount, inactive: 'no' });
    expect(result).toBeFailureWithErrors([AccountErrors.invalidInactive]);
  });

  it('rejects invalid id', () => {
    const result = Account.create({ ...validAccount, id: 'invalid-id' });
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('accepts empty description', () => {
    const result = Account.create({ ...validAccount, description: '' });
    expect(result).toBeSuccessWithValue(expect.objectContaining({ description: '' }));
  });
});
