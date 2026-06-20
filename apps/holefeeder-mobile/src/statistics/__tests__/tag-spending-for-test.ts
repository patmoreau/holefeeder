import { Money } from '@holefeeder/shared/core';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { aString } from '@/shared/__tests__/string-for-test';
import { TagSpending } from '@/statistics/core/tag-spending';

const defaultTagSpending = (): TagSpending => ({
  tag: aString(),
  spentAmount: Money.fromCents(anAmount()),
});

export const aTagSpending = (overrides?: Partial<TagSpending>): TagSpending => ({
  ...defaultTagSpending(),
  ...overrides,
});
