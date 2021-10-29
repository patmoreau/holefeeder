import { IAccount } from '../interfaces/account.interface';

export class Account implements IAccount {
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
