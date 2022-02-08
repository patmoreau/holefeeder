import { Injectable } from "@angular/core";
import { dateFromUtc } from "@app/shared/date-parser.helper";
import { IAccountInfo } from "@app/shared/interfaces/account-info.interface";
import { Adapter } from "@app/shared/interfaces/adapter.interface";
import { ICashflowInfo } from "@app/shared/interfaces/cashflow-info.interface";
import { ICategoryInfo } from "@app/shared/interfaces/category-info.interface";

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
  ) { }
}

@Injectable()
export class TransactionDetailAdapter implements Adapter<TransactionDetail> {
  adapt(item: any): TransactionDetail {
    return new TransactionDetail(item.id, dateFromUtc(item.date), item.amount, item.description,
      item.category, item.account, item.cashflow, item.tags);
  }
}
