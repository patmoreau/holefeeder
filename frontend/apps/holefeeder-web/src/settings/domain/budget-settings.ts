import { type DateIntervalType, DateOnly, type Settings } from '@holefeeder/shared/core';

const valid = (value: Record<string, unknown>): Settings => ({
  effectiveDate: DateOnly.valid(value.effectiveDate),
  intervalType: value.intervalType as DateIntervalType,
  frequency: value.frequency as number,
});

export const BudgetSettings = {
  valid: valid,
};
