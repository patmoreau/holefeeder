import { AccountErrors } from '@/accounts/core/account';
import { AccountType } from '@/accounts/core/account-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';
import { Variation } from '@/shared/core/variation';

export type UpdateAccountCommand = {
  id: Id;
  type: AccountType;
  name: string;
  openBalance: Variation;
  openDate: DateOnly;
  description: string;
  favorite: boolean;
  inactive: boolean;
};

const isValidName = Validator.string({ minLength: 1 });
const isValidDescription = Validator.string();
const isValidBoolean = Validator.boolean();

const create = (value: Record<string, unknown>): Result<UpdateAccountCommand> =>
  Result.combine<UpdateAccountCommand>({
    id: Id.create(value.id),
    type: AccountType.create(value.type),
    name: Validate.validate(isValidName, value.name, [AccountErrors.invalidName]),
    openBalance: Variation.create(value.openBalance),
    openDate: DateOnly.create(value.openDate),
    description: Validate.validate(isValidDescription, value.description, [AccountErrors.invalidDescription]),
    favorite: Validate.validate(isValidBoolean, value.favorite, [AccountErrors.invalidFavorite]),
    inactive: Validate.validate(isValidBoolean, value.inactive, [AccountErrors.invalidInactive]),
  });

export const UpdateAccountCommand = {
  create,
};
