import { DateOnly, Id, Result, Validate, Validator, Variation } from '@holefeeder/core';
import { AccountType } from '@/accounts/core/account-type';

export type Account = {
  id: Id;
  type: AccountType;
  name: string;
  openBalance: Variation;
  openDate: string;
  description: string;
  favorite: boolean;
  inactive: boolean;
};

export const AccountErrors = {
  invalidName: 'invalid-name',
  invalidOpenBalance: 'invalid-open-balance',
  invalidOpenDate: 'invalid-open-date',
  invalidDescription: 'invalid-description',
  invalidFavorite: 'invalid-favorite',
  invalidInactive: 'invalid-inactive',
};

const isValidName = Validator.string({ minLength: 1 });
const isValidDescription = Validator.string();
const isValidBoolean = Validator.boolean();

const create = (value: Record<string, unknown>): Result<Account> =>
  Result.combine<Account>({
    id: Id.create(value.id),
    type: AccountType.create(value.type),
    name: Validate.validate(isValidName, value.name, [AccountErrors.invalidName]),
    openBalance: Variation.create(value.openBalance),
    openDate: DateOnly.create(value.openDate),
    description: Validate.validate(isValidDescription, value.description, [AccountErrors.invalidDescription]),
    favorite: Validate.validate(isValidBoolean, value.favorite, [AccountErrors.invalidFavorite]),
    inactive: Validate.validate(isValidBoolean, value.inactive, [AccountErrors.invalidInactive]),
  });

const valid = (value: Record<string, unknown>): Account => ({
  id: Id.valid(value.id),
  type: AccountType.valid(value.type),
  name: value.name as string,
  openBalance: Variation.valid(value.openBalance),
  openDate: DateOnly.valid(value.openDate),
  description: value.description as string,
  favorite: value.favorite as boolean,
  inactive: value.inactive as boolean,
});

export const Account = {
  create: create,
  valid: valid,
};
