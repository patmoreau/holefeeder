import { Money } from '@holefeeder/shared/core';
import { anAmount, aString } from '@holefeeder/shared/testkit';
import { TagIncome } from '@/statistics/core/tag-income';

const defaultTagIncome = (): TagIncome => ({
  tag: aString(),
  receivedAmount: Money.fromCents(anAmount()),
  avgAmount: Money.ZERO,
});

export const aTagIncome = (overrides?: Partial<TagIncome>): TagIncome => ({
  ...defaultTagIncome(),
  ...overrides,
});
