import { IAccountInfo } from "../interfaces/account-info.interface";
import { ICashflowInfo } from "../interfaces/cashflow-info.interface";
import { ICategoryInfo } from "../interfaces/category-info.interface";
import { ITransactionDetail } from "../interfaces/transaction-detail.interface";

export class TransactionDetail implements ITransactionDetail {
  id: string;
  date: Date;
  amount: number;
  description: string;
  category: ICategoryInfo;
  account: IAccountInfo;
  cashflow: ICashflowInfo;
  tags: string[];
}
