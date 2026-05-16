import { DateIntervalType, DateOnly } from '@holefeeder/core';

export type SettingsFormData = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};
