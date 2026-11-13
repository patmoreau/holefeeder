import { Favorite, Money } from '@holefeeder/shared/core';
import { aCategoryType, aColor, anId, aString } from '@holefeeder/shared/testkit';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';

const defaultCommand = (): UpdateCategoryCommand => ({
  id: anId(),
  name: aString(),
  type: aCategoryType(),
  color: aColor(),
  budgetAmount: Money.ZERO,
  favorite: false as Favorite,
});

export const anUpdateCategoryCommand = (overrides?: Partial<UpdateCategoryCommand>): UpdateCategoryCommand => ({
  ...defaultCommand(),
  ...overrides,
});
