import { DateInterval } from '@holefeeder/core';
import { aPastDate, aRecentDate } from '@/shared/__tests__/date-for-test';

const defaultDateInterval = (): DateInterval => ({
  start: aPastDate(),
  end: aRecentDate(),
});

export const aDateInterval = (overrides?: Partial<DateInterval>): DateInterval => ({
  ...defaultDateInterval(),
  ...overrides,
});
