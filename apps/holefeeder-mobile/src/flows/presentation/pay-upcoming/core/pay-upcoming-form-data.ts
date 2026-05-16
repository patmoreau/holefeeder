import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';

export type PayUpcomingFormData = {
  cashflowId: Id;
  cashflowDate: DateOnly;
  date: DateOnly;
  amount: number;
};
