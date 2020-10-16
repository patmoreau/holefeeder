
import { AccountType } from '../enums/account-type.enum';
import { IAccountDetail } from '../interfaces/account-detail.interface';

export class AccountDetail implements IAccountDetail {
  id: string;
  name: string;
  type: AccountType;
  transactionCount: number;
  balance: number;
  updated: Date;
  description: string;
  favorite: boolean;
  inactive: boolean;
}
