import { Favorite, Money } from '@holefeeder/shared/core';
import { aCategoryType, aColor, aString } from '@holefeeder/shared/testkit';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';

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
