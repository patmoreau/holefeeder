import { DateOnly, Favorite, Id, Inactive, Result, Validate, Validator, Variation } from '@holefeeder/shared/core';
import { AccountErrors } from '@/accounts/core/account';
import { AccountType } from '@/accounts/core/account-type';

export type UpdateAccountCommand = {
  id: Id;
  type: AccountType;
  name: string;
  openBalance: Variation;
  openDate: DateOnly;
  description: string;
  favorite: Favorite;
  inactive: Inactive;
};

const isValidName = Validator.string({ minLength: 1 });
const isValidDescription = Validator.string();

const create = (value: Record<string, unknown>): Result<UpdateAccountCommand> =>
  Result.combine<UpdateAccountCommand>({
    id: Id.create(value.id),
    type: AccountType.create(value.type),
    name: Validate.validate(isValidName, value.name, [AccountErrors.invalidName]),
    openBalance: Variation.create(value.openBalance),
    openDate: DateOnly.create(value.openDate),
    description: Validate.validate(isValidDescription, value.description, [AccountErrors.invalidDescription]),
    favorite: Favorite.create(value.favorite),
    inactive: Inactive.create(value.inactive),
  });

export const UpdateAccountCommand = {
  create,
};
