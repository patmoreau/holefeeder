import { DateIntervalType } from '../enums/date-interval-type.enum';
import { IAccountInfo } from '../interfaces/account-info.interface';
import { ICashflowDetail } from '../interfaces/cashflow-detail.interface';
import { ICategoryInfo } from '../interfaces/category-info.interface';

export class CashflowDetail implements ICashflowDetail {
  id: string;
  effectiveDate: Date;
  amount: number;
  intervalType: DateIntervalType;
  frequency: number;
  recurrence: number;
  description: string;
  category: ICategoryInfo;
  account: IAccountInfo;
  tags: string[];
}
