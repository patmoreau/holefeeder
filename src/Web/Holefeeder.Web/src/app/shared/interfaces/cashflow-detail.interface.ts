import { ICategoryInfo } from './category-info.interface';
import { IAccountInfo } from './account-info.interface';
import { CashflowDetail } from '../models/cashflow-detail.model';
import { dateFromUtc } from '../date-parser.helper';
import { DateIntervalType } from '../enums/date-interval-type.enum';

export interface ICashflowDetail {
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

export function cashflowDetailFromServer(item: ICashflowDetail): ICashflowDetail {
  return Object.assign(new CashflowDetail(), item, {
    effectiveDate: dateFromUtc(item.effectiveDate),
  });
}
