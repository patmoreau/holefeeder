import { Money } from '@holefeeder/shared/core';
import { anAmount, aString } from '@holefeeder/shared/testkit';
import { TagSpending } from '@/statistics/core/tag-spending';

const defaultTagSpending = (): TagSpending => ({
  tag: aString(),
  spentAmount: Money.fromCents(anAmount()),
  avgAmount: Money.ZERO,
});

export const aTagSpending = (overrides?: Partial<TagSpending>): TagSpending => ({
  ...defaultTagSpending(),
  ...overrides,
});
