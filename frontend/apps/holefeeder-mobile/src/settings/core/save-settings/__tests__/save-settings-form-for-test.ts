import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { aDateIntervalType } from '@/shared/__tests__/enum-for-test';
import { aCount } from '@/shared/__tests__/number-for-test';

const defaultSaveSettingsForm = (): Record<string, unknown> => ({
  effectiveDate: aRecentDate(),
  intervalType: aDateIntervalType(),
  frequency: aCount() + 1,
});

export const aSaveSettingsForm = (overrides?: Record<string, unknown>): Record<string, unknown> => {
  return {
    ...defaultSaveSettingsForm(),
    ...overrides,
  };
};
