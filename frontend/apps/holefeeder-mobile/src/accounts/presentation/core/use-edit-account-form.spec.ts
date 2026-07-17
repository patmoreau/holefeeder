import { DateOnly, Id } from '@holefeeder/shared/core';
import { AccountTypes } from '@/accounts/core/account-type';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { EditAccountFormError, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';

const aFormData = (overrides?: Partial<EditAccountFormData>): EditAccountFormData => ({
  id: Id.valid('00000000-0000-0000-0000-000000000001'),
  name: 'Chequing',
  type: AccountTypes.checking,
  openBalance: 100,
  openDate: DateOnly.valid('2023-01-15'),
  description: 'a description',
  favorite: false,
  inactive: false,
  ...overrides,
});

describe('EditAccountFormError', () => {
  it('should have nameRequired error constant', () => {
    expect(EditAccountFormError.nameRequired).toBe('nameRequired');
  });
});

describe('validateEditAccountForm', () => {
  it('should return no errors for valid form data', () => {
    const errors = validateEditAccountForm(aFormData());

    expect(errors).toEqual({});
  });

  it('should return nameRequired error when name is empty', () => {
    const errors = validateEditAccountForm(aFormData({ name: '' }));

    expect(errors.name).toBe(EditAccountFormError.nameRequired);
    expect(Object.keys(errors).length).toBe(1);
  });

  it('should return nameRequired error when name is only whitespace', () => {
    const errors = validateEditAccountForm(aFormData({ name: '   ' }));

    expect(errors.name).toBe(EditAccountFormError.nameRequired);
  });

  it('should not return error for a name with surrounding whitespace', () => {
    const errors = validateEditAccountForm(aFormData({ name: '  Savings  ' }));

    expect(errors.name).toBeUndefined();
  });
});
