import {Injectable} from "@angular/core";
import {dateToUtc} from "@app/shared/date-parser.helper";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export class PayCashflowCommand {
  constructor(
    public date: Date,
    public amount: number,
    public cashflowId: string,
    public cashflowDate: Date
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class PayCashflowCommandAdapter implements Adapter<PayCashflowCommand> {
  adapt(item: any): PayCashflowCommand {
    return new PayCashflowCommand(dateToUtc(item.date), item.amount, item.cashflow, dateToUtc(item.cashflowDate));
  }
}
