import { DateInterval } from '../core';
import { aPastDate, aRecentDate } from './date-for-test';

const defaultDateInterval = (): DateInterval => ({
  start: aPastDate(),
  end: aRecentDate(),
});

export const aDateInterval = (overrides?: Partial<DateInterval>): DateInterval => ({
  ...defaultDateInterval(),
  ...overrides,
});
