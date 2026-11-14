import { Settings } from '../core';
import { aPastDate } from './date-for-test';
import { aDateIntervalType } from './enum-for-test';
import { aCount } from './number-for-test';

const defaultSettings = (): Settings => ({
  effectiveDate: aPastDate(),
  intervalType: aDateIntervalType(),
  frequency: aCount() + 1,
});

export const aSettings = (overrides?: Partial<Settings>): Settings => ({
  ...defaultSettings(),
  ...overrides,
});
