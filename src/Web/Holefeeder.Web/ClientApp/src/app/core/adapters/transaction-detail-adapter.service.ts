import { Injectable } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter, CategoryType, TransactionDetail } from '@app/shared/models';

export type transactionDetailType = {
  id: string;
  date: Date;
  amount: number;
  description: string;
  category: {
    id: string;
    name: string;
    type: CategoryType;
    color: string;
  };
  account: {
    id: string;
    name: string;
    mongoId: string;
  };
  cashflow: {
    id: string;
    date: Date;
  };
  tags: string[];
};

@Injectable({ providedIn: 'root' })
export class TransactionDetailAdapter implements Adapter<TransactionDetail> {
  adapt(item: transactionDetailType): TransactionDetail {
    return new TransactionDetail(
      item.id,
      dateFromUtc(item.date),
      item.amount,
      item.description,
      item.category,
      item.account,
      item.cashflow,
      item.tags
    );
  }
}
