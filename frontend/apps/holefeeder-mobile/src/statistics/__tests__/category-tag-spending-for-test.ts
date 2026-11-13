import { anAmount, anId, aString } from '@holefeeder/shared/testkit';
import { CategoryTagSpending } from '@/statistics/core/category-tag-spending';

const defaultCategoryTagSpending = (): CategoryTagSpending => ({
  categoryId: anId(),
  tag: aString(),
  spentAmount: anAmount(),
});

export const aCategoryTagSpending = (overrides?: Partial<CategoryTagSpending>): CategoryTagSpending => ({
  ...defaultCategoryTagSpending(),
  ...overrides,
});
