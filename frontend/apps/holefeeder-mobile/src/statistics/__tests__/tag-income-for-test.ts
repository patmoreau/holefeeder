import { Money } from '@holefeeder/shared/core';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { aString } from '@/shared/__tests__/string-for-test';
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
