import { Injectable } from '@angular/core';
import { CashflowRequestAdapter } from '@app/core/adapters';
import { dateToUtc } from '@app/shared/helpers';
import {
  Adapter,
  DateIntervalType,
  MakePurchaseCommand,
} from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class MakePurchaseCommandAdapter
  implements Adapter<MakePurchaseCommand>
{
  constructor(private adapter: CashflowRequestAdapter) {}

  adapt(item: {
    date: Date;
    amount: number;
    description: string;
    account: string;
    category: string;
    tags: string[];
    cashflow: {
      effectiveDate: Date;
      intervalType: DateIntervalType;
      frequency: number;
      recurrence: number;
    } | null;
  }): MakePurchaseCommand {
    return new MakePurchaseCommand(
      dateToUtc(item.date),
      item.amount,
      item.description,
      item.account,
      item.category,
      item.tags,
      item.cashflow !== null ? this.adapter.adapt(item.cashflow) : null
    );
  }
}
