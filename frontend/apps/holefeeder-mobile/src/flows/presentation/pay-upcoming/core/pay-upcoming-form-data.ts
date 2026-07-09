import { DateOnly, Id } from '@holefeeder/shared/core';

export type PayUpcomingFormData = {
  cashflowId: Id;
  cashflowDate: DateOnly;
  date: DateOnly;
  amount: number;
};
