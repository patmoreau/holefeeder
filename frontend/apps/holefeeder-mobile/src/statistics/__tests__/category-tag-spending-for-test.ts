import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
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
