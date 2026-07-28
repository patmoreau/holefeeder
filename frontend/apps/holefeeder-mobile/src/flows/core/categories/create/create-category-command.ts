import { Favorite, Money, Result, Validate, Validator } from '@holefeeder/shared/core';
import { CategoryErrors } from '@/flows/core/categories/category';
import { CategoryType } from '@/shared/core/category-type';

export type CreateCategoryCommand = {
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: Money;
  favorite: Favorite;
};

const isValidName = Validator.string({ minLength: 1 });
const isValidColor = Validator.string({ minLength: 1 });

const create = (value: Record<string, unknown>): Result<CreateCategoryCommand> =>
  Result.combine<CreateCategoryCommand>({
    name: Validate.validate(isValidName, value.name, [CategoryErrors.invalidName]),
    type: CategoryType.create(value.type),
    color: Validate.validate(isValidColor, value.color, [CategoryErrors.invalidColor]),
    budgetAmount: Money.create(value.budgetAmount),
    favorite: Favorite.create(value.favorite),
  });

export const CreateCategoryCommand = {
  create,
};
