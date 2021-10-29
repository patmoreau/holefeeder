import { ICategoryInfo } from './category-info.interface';
import { IAccountInfo } from './account-info.interface';
import { ICashflowInfo } from './cashflow-info.interface';
import { TransactionDetail } from '../models/transaction-detail.model';
import { dateFromUtc } from '../date-parser.helper';

export interface ITransactionDetail {
    id: string;
    date: Date;
    amount: number;
    description: string;
    category: ICategoryInfo;
    account: IAccountInfo;
    cashflow: ICashflowInfo;
    tags: string[];
}

export function transactionDetailFromServer(item: ITransactionDetail): ITransactionDetail {
  return Object.assign(new TransactionDetail(), item, {
      date: dateFromUtc(item.date),
      //cashflowDate: dateFromUtc(item.cashflowDate)
  });
}
