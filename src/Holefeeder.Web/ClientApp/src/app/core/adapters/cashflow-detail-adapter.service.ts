import { Injectable } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import {
  Adapter,
  CashflowDetail,
  DateIntervalType,
  IAccountInfo,
  ICategoryInfo,
} from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class CashflowDetailAdapter implements Adapter<CashflowDetail> {
  adapt(item: {
    id: string;
    effectiveDate: Date;
    amount: number;
    intervalType: DateIntervalType;
    frequency: number;
    recurrence: number;
    description: string;
    category: ICategoryInfo;
    account: IAccountInfo;
    inactive: boolean;
    tags: string[];
  }): CashflowDetail {
    return new CashflowDetail(
      item.id,
      dateFromUtc(item.effectiveDate),
      item.amount,
      item.intervalType,
      item.frequency,
      item.recurrence,
      item.description,
      item.category,
      item.account,
      item.inactive,
      item.tags
    );
  }
}
