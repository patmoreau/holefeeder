import { Favorite, Id, Money, Result, Validate, Validator } from '@holefeeder/shared/core';
import { CategoryType } from '@/shared/core/category-type';
import { System } from '@/shared/core/system';

export type Category = {
  id: Id;
  name: string;
  type: CategoryType;
  color: string;
  budgetAmount: Money;
  favorite: Favorite;
  system: System;
};

export const CategoryErrors = {
  invalidName: 'invalid-name',
  invalidColor: 'invalid-color',
  invalidBudgetAmount: 'invalid-budget-amount',
};

const isValidName = Validator.string({ minLength: 1 });
const isValidColor = Validator.string({ minLength: 1 });

const create = (value: Record<string, unknown>): Result<Category> =>
  Result.combine<Category>({
    id: Id.create(value.id),
    type: CategoryType.create(value.type),
    name: Validate.validate(isValidName, value.name, [CategoryErrors.invalidName]),
    color: Validate.validate(isValidColor, value.color, [CategoryErrors.invalidColor]),
    budgetAmount: Money.create(value.budgetAmount),
    favorite: Favorite.create(value.favorite),
    system: System.create(value.system),
  });

const valid = (value: Record<string, unknown>): Category => ({
  id: Id.valid(value.id),
  type: CategoryType.valid(value.type),
  name: value.name as string,
  color: value.color as string,
  budgetAmount: Money.valid(value.budgetAmount),
  favorite: value.favorite as Favorite,
  system: value.system as System,
});

export const Category = {
  create: create,
  valid: valid,
};
