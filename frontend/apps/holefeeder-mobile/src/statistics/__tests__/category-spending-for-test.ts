import { Money } from '@holefeeder/shared/core';
import { anAmount, aColor, anId, aString } from '@holefeeder/shared/testkit';
import { CategorySpending } from '@/statistics/core/category-spending';

const defaultCategorySpending = (): CategorySpending => ({
  categoryId: anId(),
  categoryName: aString(),
  color: aColor(),
  budgetAmount: Money.ZERO,
  spentAmount: anAmount(),
  avgAmount: Money.ZERO,
});

export const aCategorySpending = (overrides?: Partial<CategorySpending>): CategorySpending => ({
  ...defaultCategorySpending(),
  ...overrides,
});
