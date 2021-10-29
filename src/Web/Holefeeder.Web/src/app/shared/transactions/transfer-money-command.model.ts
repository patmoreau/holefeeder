export class TransferMoneyCommand {
  date: Date;
  amount: number;
  description: string;
  fromAccountId: string;
  toAccountId: string;

  constructor(obj: {date: Date, amount: number, description: string, fromAccountId: string,
    toAccountId: string}) {
    this.date = obj.date;
    this.amount = obj.amount;
    this.description = obj.description;
    this.fromAccountId = obj.fromAccountId;
    this.toAccountId = obj.toAccountId;
  }
}
