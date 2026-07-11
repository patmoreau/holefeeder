import { Favorite, Money } from '@holefeeder/shared/core';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';
import { aCategoryType } from '@/shared/__tests__/enum-for-test';
import { aColor, aString } from '@/shared/__tests__/string-for-test';

const defaultCommand = (): CreateCategoryCommand => ({
  name: aString(),
  type: aCategoryType(),
  color: aColor(),
  budgetAmount: Money.ZERO,
  favorite: false as Favorite,
});

export const aCreateCategoryCommand = (overrides?: Partial<CreateCategoryCommand>): CreateCategoryCommand => ({
  ...defaultCommand(),
  ...overrides,
});
