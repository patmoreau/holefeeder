import {Injectable} from "@angular/core";
import {Adapter, dateToUtc} from "@app/shared";

export class TransferMoneyCommand {
  constructor(
    public date: Date,
    public amount: number,
    public description: string,
    public fromAccountId: string,
    public toAccountId: string
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class TransferMoneyCommandAdapter implements Adapter<TransferMoneyCommand> {
  adapt(item: any): TransferMoneyCommand {
    return new TransferMoneyCommand(dateToUtc(item.date), item.amount, item.description, item.fromAccount, item.toAccount);
  }
}
