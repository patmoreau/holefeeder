import { aRecentDate, aDateIntervalType, aCount } from '@holefeeder/shared/testkit';

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
