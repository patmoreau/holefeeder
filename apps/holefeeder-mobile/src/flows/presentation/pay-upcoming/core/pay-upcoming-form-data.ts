import { DateOnly, Id } from '@holefeeder/core';

export type PayUpcomingFormData = {
  cashflowId: Id;
  cashflowDate: DateOnly;
  date: DateOnly;
  amount: number;
};
