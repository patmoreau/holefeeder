import { DateOnly, Favorite, Id, Inactive, Result, Validate, Validator, Variation } from '@holefeeder/shared/core';
import { AccountType } from '@/accounts/core/account-type';

export type Account = {
  id: Id;
  type: AccountType;
  name: string;
  openBalance: Variation;
  openDate: string;
  description: string;
  favorite: Favorite;
  inactive: Inactive;
};

export const AccountErrors = {
  invalidName: 'invalid-name',
  invalidOpenBalance: 'invalid-open-balance',
  invalidOpenDate: 'invalid-open-date',
  invalidDescription: 'invalid-description',
};

const isValidName = Validator.string({ minLength: 1 });
const isValidDescription = Validator.string();

const create = (value: Record<string, unknown>): Result<Account> =>
  Result.combine<Account>({
    id: Id.create(value.id),
    type: AccountType.create(value.type),
    name: Validate.validate(isValidName, value.name, [AccountErrors.invalidName]),
    openBalance: Variation.create(value.openBalance),
    openDate: DateOnly.create(value.openDate),
    description: Validate.validate(isValidDescription, value.description, [AccountErrors.invalidDescription]),
    favorite: Favorite.create(value.favorite),
    inactive: Inactive.create(value.inactive),
  });

const valid = (value: Record<string, unknown>): Account => ({
  id: Id.valid(value.id),
  type: AccountType.valid(value.type),
  name: value.name as string,
  openBalance: Variation.valid(value.openBalance),
  openDate: DateOnly.valid(value.openDate),
  description: value.description as string,
  favorite: value.favorite as Favorite,
  inactive: value.inactive as Inactive,
});

export const Account = {
  create: create,
  valid: valid,
};
