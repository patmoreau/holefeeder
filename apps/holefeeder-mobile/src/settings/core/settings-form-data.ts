import { DateIntervalType, DateOnly } from '@holefeeder/shared/core';

export type SettingsFormData = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};
