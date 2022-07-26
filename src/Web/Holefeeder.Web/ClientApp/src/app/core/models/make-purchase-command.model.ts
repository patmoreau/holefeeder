import {Injectable} from "@angular/core";
import {CashflowRequest, CashflowRequestAdapter} from "@app/core/models/cashflow-request.model";
import {Adapter, dateToUtc} from "@app/shared";

export class MakePurchaseCommand {
  constructor(
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[],
    public cashflow: CashflowRequest | null
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class MakePurchaseCommandAdapter implements Adapter<MakePurchaseCommand> {
  constructor(private adapter: CashflowRequestAdapter) {
  }

  adapt(item: any): MakePurchaseCommand {
    return new MakePurchaseCommand(dateToUtc(item.date), item.amount, item.description,
      item.account, item.category, item.tags, item.cashflow !== null ? this.adapter.adapt(item.cashflow) : null);
  }
}
