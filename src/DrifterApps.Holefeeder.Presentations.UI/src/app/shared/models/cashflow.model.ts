import { ICashflow } from '../interfaces/cashflow.interface';
import { DateIntervalType } from '../enums/date-interval-type.enum';

export class Cashflow implements ICashflow {
  id: string;
  intervalType: DateIntervalType;
  effectiveDate: Date;
  amount: number;
  frequency: number;
  recurrence: number;
  description: string;
  account: string;
  category: string;
  inactive: boolean;
  tags: Array<string>;
}
