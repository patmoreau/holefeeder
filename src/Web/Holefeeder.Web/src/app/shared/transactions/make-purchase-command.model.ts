export class MakePurchaseCommand {
  date: Date;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];

  constructor(obj: {date: Date, amount: number, description: string, accountId: string,
    categoryId: string, tags: string[]}) {
    this.date = obj.date;
    this.amount = obj.amount;
    this.description = obj.description;
    this.accountId = obj.accountId;
    this.categoryId = obj.categoryId;
    this.tags = obj.tags;
  }

}
