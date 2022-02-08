import { dateToUtc } from "../date-parser.helper";

export class PayCashflowCommand {
  date: Date;
  amount: number;
  cashflowId: string;
  cashflowDate: Date;

  constructor(obj: {date: Date, amount: number, cashflowId: string, cashflowDate: Date}) {
    this.date = dateToUtc(obj.date);
    this.amount = obj.amount;
    this.cashflowId = obj.cashflowId;
    this.cashflowDate = dateToUtc(obj.cashflowDate);
  }

}
