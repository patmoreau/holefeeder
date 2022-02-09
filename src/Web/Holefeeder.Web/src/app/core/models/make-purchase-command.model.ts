import { Injectable } from "@angular/core";
import { dateToUtc } from "@app/shared/date-parser.helper";
import { Adapter } from "@app/shared/interfaces/adapter.interface";

export class MakePurchaseCommand {
  constructor(
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[]
  ) { }
}

@Injectable({ providedIn: 'root' })
export class MakePurchaseCommandAdapter implements Adapter<MakePurchaseCommand> {
  adapt(item: any): MakePurchaseCommand {
    return new MakePurchaseCommand(dateToUtc(item.date), item.amount, item.description,
      item.account, item.category, item.tags);
  }
}
