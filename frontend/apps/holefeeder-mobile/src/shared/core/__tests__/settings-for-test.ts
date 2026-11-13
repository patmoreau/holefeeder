import { aPastDate, aDateIntervalType, aCount } from '@holefeeder/shared/testkit';
import { Settings } from '@/shared/core/settings';

const defaultSettings = (): Settings => ({
  effectiveDate: aPastDate(),
  intervalType: aDateIntervalType(),
  frequency: aCount() + 1,
});

export const aSettings = (overrides?: Partial<Settings>): Settings => ({
  ...defaultSettings(),
  ...overrides,
});
