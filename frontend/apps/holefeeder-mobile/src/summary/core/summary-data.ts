import { CategoryType, DateOnly, Money } from '@holefeeder/shared/core';

export type SummaryData = {
  type: CategoryType;
  date: DateOnly;
  total: Money;
};

const valid = (value: Record<string, unknown>): SummaryData => {
  return {
    type: CategoryType.valid(value.type),
    date: DateOnly.valid(value.date),
    total: Money.valid(value.total),
  };
};

export const SummaryData = {
  valid: valid,
};
