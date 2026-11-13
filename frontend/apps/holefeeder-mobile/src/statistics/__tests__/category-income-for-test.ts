import { Money } from '@holefeeder/shared/core';
import { anAmount, aColor, anId, aString } from '@holefeeder/shared/testkit';
import { CategoryIncome } from '@/statistics/core/category-income';

const defaultCategoryIncome = (): CategoryIncome => ({
  categoryId: anId(),
  categoryName: aString(),
  color: aColor(),
  receivedAmount: anAmount(),
  avgAmount: Money.ZERO,
});

export const aCategoryIncome = (overrides?: Partial<CategoryIncome>): CategoryIncome => ({
  ...defaultCategoryIncome(),
  ...overrides,
});
