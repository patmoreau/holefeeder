import { aPastDate, aRecentDate } from '@/shared/__tests__/date-for-test';
import { DateInterval } from '@/shared/core/date-interval';

const defaultDateInterval = (): DateInterval => ({
  start: aPastDate(),
  end: aRecentDate(),
});

export const aDateInterval = (overrides?: Partial<DateInterval>): DateInterval => ({
  ...defaultDateInterval(),
  ...overrides,
});
