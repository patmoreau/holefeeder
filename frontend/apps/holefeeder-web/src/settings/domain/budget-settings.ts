import { DateOnly } from '@holefeeder/shared/core';

export type BudgetIntervalType = 'daily' | 'weekly' | 'monthly' | 'yearly' | 'oneTime';

export type BudgetSettings = {
  effectiveDate: DateOnly;
  intervalType: BudgetIntervalType;
  frequency: number;
};

const valid = (value: Record<string, unknown>): BudgetSettings => ({
  effectiveDate: DateOnly.valid(value.effectiveDate),
  intervalType: value.intervalType as BudgetIntervalType,
  frequency: value.frequency as number,
});

export const BudgetSettings = {
  valid: valid,
};
