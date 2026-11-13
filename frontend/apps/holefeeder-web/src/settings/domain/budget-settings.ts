import { type DateIntervalType, DateOnly } from '@holefeeder/shared/core';

export type BudgetSettings = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};

const valid = (value: Record<string, unknown>): BudgetSettings => ({
  effectiveDate: DateOnly.valid(value.effectiveDate),
  intervalType: value.intervalType as DateIntervalType,
  frequency: value.frequency as number,
});

export const BudgetSettings = {
  valid: valid,
};
