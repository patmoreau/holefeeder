import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';

export type SettingsFormData = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};
