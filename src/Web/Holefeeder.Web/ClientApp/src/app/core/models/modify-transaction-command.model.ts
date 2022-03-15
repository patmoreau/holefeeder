import {Injectable} from "@angular/core";
import {dateToUtc} from "@app/shared/date-parser.helper";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export class ModifyTransactionCommand {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[]
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class ModifyTransactionCommandAdapter implements Adapter<ModifyTransactionCommand> {
  adapt(item: any): ModifyTransactionCommand {
    return new ModifyTransactionCommand(item.id, dateToUtc(item.date), item.amount, item.description,
      item.account, item.category, item.tags);
  }
}
