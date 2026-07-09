import { Money } from '@holefeeder/shared/core';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { aColor, anId, aString } from '@/shared/__tests__/string-for-test';
import { CategorySpending } from '@/statistics/core/category-spending';

const defaultCategorySpending = (): CategorySpending => ({
  categoryId: anId(),
  categoryName: aString(),
  color: aColor(),
  budgetAmount: Money.ZERO,
  spentAmount: anAmount(),
});

export const aCategorySpending = (overrides?: Partial<CategorySpending>): CategorySpending => ({
  ...defaultCategorySpending(),
  ...overrides,
});
