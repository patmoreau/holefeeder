import { dateToUtc } from "../date-parser.helper";

export class ModifyTransactionCommand {
  id: string;
  date: Date;
  amount: number;
  description: string;
  accountId: string;
  categoryId: string;
  tags: string[];

  constructor(obj: {
    id: string, date: Date, amount: number, description: string, accountId: string,
    categoryId: string, tags: string[]
  }) {
    this.id = obj.id;
    this.date = dateToUtc(obj.date);
    this.amount = obj.amount;
    this.description = obj.description;
    this.accountId = obj.accountId;
    this.categoryId = obj.categoryId;
    this.tags = obj.tags;
  }

}
