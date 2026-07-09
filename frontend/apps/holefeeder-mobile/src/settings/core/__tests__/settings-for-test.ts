import { Settings } from '@/settings/core/settings';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aDateIntervalType } from '@/shared/__tests__/enum-for-test';
import { aCount } from '@/shared/__tests__/number-for-test';

const defaultSettings = (): Settings => ({
  effectiveDate: aPastDate(),
  intervalType: aDateIntervalType(),
  frequency: aCount() + 1,
});

export const aSettings = (overrides?: Partial<Settings>): Settings => ({
  ...defaultSettings(),
  ...overrides,
});
