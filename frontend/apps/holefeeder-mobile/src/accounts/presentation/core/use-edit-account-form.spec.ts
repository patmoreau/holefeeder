import { DateOnly, DateOnlyErrors, Id, Variation } from '@holefeeder/shared/core';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { AccountTypes } from '@/accounts/core/account-type';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { EditAccountFormError, saveAccount, validateEditAccountForm } from '@/accounts/presentation/core/use-edit-account-form';
import { aRepositoriesState } from '@/shared/repositories/__tests__/repositories-state-for-test';

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

describe('saveAccount', () => {
  let accountRepository: AccountsRepositoryInMemory;

  beforeEach(() => {
    accountRepository = AccountsRepositoryInMemory();
  });

  it('maps the form data to an update command and saves it', async () => {
    const formData = aFormData();
    accountRepository.add(anAccount({ id: formData.id }));

    const result = await saveAccount(aRepositoriesState({ accountRepository }), formData);

    expect(result).toBeSuccessWithValue(formData.id);
    expect(accountRepository.updatedCommands()).toEqual([
      {
        id: formData.id,
        type: formData.type,
        name: formData.name,
        openBalance: Variation.valid(formData.openBalance),
        openDate: formData.openDate,
        description: formData.description,
        favorite: formData.favorite,
        inactive: formData.inactive,
      },
    ]);
  });

  it('fails without touching the repository when the command is invalid', async () => {
    const formData = aFormData({ openDate: DateOnly.valid('not-a-date') });

    const result = await saveAccount(aRepositoriesState({ accountRepository }), formData);

    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
    expect(accountRepository.updatedCommands()).toEqual([]);
  });

  it('propagates a repository failure after dispatching the command', async () => {
    const formData = aFormData();
    accountRepository.add(anAccount({ id: formData.id }));
    accountRepository.isFailing(['boom']);

    const result = await saveAccount(aRepositoriesState({ accountRepository }), formData);

    expect(result).toBeFailureWithErrors(['boom']);
    expect(accountRepository.updatedCommands()).toHaveLength(1);
  });
});
