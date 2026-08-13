import { DateOnly, Favorite, Result, Validate, Validator, Variation } from '@holefeeder/shared/core';
import { AccountErrors } from '@/accounts/core/account';
import { AccountType } from '@/accounts/core/account-type';

// No id: the repository mints one. No inactive either — an account is not created
// switched off.
export type CreateAccountCommand = {
  type: AccountType;
  name: string;
  openBalance: Variation;
  openDate: DateOnly;
  description: string;
  favorite: Favorite;
};

const isValidName = Validator.string({ minLength: 1 });
const isValidDescription = Validator.string();

const create = (value: Record<string, unknown>): Result<CreateAccountCommand> =>
  Result.combine<CreateAccountCommand>({
    type: AccountType.create(value.type),
    name: Validate.validate(isValidName, value.name, [AccountErrors.invalidName]),
    openBalance: Variation.create(value.openBalance),
    openDate: DateOnly.create(value.openDate),
    description: Validate.validate(isValidDescription, value.description, [AccountErrors.invalidDescription]),
    favorite: Favorite.create(value.favorite),
  });

export const CreateAccountCommand = {
  create,
};
