import { Injectable } from '@angular/core';
import { TransactionDetail } from '@app/core/models/transaction-detail.model';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

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
