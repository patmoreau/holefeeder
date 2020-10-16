import { IAccount } from '../interfaces/account.interface';

export class Account implements IAccount {
  id: string;
  name: string;
  type: string;
  openBalance: number;
  openDate: Date;
  description: string;
  favorite: boolean;
  inactive: boolean;
}
