import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { dateFromUtc } from "@app/shared/helpers";
import { TransactionDetail } from "@app/core/models/transaction-detail.model";

@Injectable({ providedIn: "root" })
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
