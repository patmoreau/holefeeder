import { DateOnly, Money } from '@holefeeder/shared/core';

export type BillsDueTotal = {
  count: number;
  amount: Money;
};

export type BillsDueSummary = {
  pastDue: BillsDueTotal;
  dueToday: BillsDueTotal;
};

export type BillsDueEntry = {
  date: DateOnly;
  summary: BillsDueSummary;
};
