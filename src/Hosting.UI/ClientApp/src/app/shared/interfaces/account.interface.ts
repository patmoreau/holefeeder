import { dateToUtc, dateFromUtc } from '../date-parser.helper';
import { Account } from '../models/account.model';

export interface IAccount {
  id: string;
  name: string;
  type: string;
  openBalance: number;
  openDate: Date;
  description: string;
  favorite: boolean;
  inactive: boolean;
}

export function accountToServer(item: IAccount): IAccount {
  return Object.assign({} as IAccount, item, {
    openDate: dateToUtc(item.openDate)
  });
}

export function accountFromServer(item: IAccount): IAccount {
  return Object.assign(new Account(), item, {
    openDate: dateFromUtc(item.openDate)
  });
}
