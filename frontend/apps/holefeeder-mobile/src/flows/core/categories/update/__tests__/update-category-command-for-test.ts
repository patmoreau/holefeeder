import { Favorite, Money } from '@holefeeder/shared/core';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';
import { aCategoryType } from '@/shared/__tests__/enum-for-test';
import { aColor, anId, aString } from '@/shared/__tests__/string-for-test';

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
