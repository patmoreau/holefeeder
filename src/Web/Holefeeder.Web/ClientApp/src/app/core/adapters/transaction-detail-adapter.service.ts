import { Injectable } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter, TransactionDetail } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class TransactionDetailAdapter implements Adapter<TransactionDetail> {
  adapt(item: any): TransactionDetail {
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
