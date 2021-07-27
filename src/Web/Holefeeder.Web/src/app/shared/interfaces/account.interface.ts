import { dateToUtc, dateFromUtc } from '../date-parser.helper';
import { Account } from '../models/account.model';

export interface IAccount {
  id: string;
  name: string;
  type: string;
  openBalance: number;
  openDate: Date;
  transactionCount: number;
  balance: number;
  updated: Date;
  description: string;
  favorite: boolean;
}

export function accountDetailToServer(item: IAccount): IAccount {
  return Object.assign({} as IAccount, item, {
    updated: dateToUtc(item.updated),
    openDate: dateToUtc(item.openDate)
  });
}

export function accountDetailFromServer(item: IAccount): IAccount {
  return Object.assign(new Account(), item, {
      updated: dateFromUtc(item.updated),
      openDate: dateFromUtc(item.openDate)
  });
}
