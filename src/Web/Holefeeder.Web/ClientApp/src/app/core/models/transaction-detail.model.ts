import {Injectable} from "@angular/core";
import {Adapter, dateFromUtc, IAccountInfo, ICashflowInfo, ICategoryInfo} from "@app/shared";

export class TransactionDetail {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public category: ICategoryInfo,
    public account: IAccountInfo,
    public cashflow: ICashflowInfo,
    public tags: string[]
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class TransactionDetailAdapter implements Adapter<TransactionDetail> {
  adapt(item: any): TransactionDetail {
    return new TransactionDetail(item.id, dateFromUtc(item.date), item.amount, item.description,
      item.category, item.account, item.cashflow, item.tags);
  }
}
