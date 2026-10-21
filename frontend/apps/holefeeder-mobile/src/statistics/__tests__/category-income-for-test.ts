import { Money } from '@holefeeder/shared/core';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { aColor, anId, aString } from '@/shared/__tests__/string-for-test';
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
