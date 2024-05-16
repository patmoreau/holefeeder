import { Injectable } from '@angular/core';
import { Account, AccountType, Adapter } from '@app/shared/models';

export type accountType = {
  id: string;
  name: string;
  type: AccountType;
  openBalance: number;
  openDate: Date;
  transactionCount: number;
  balance: number;
  updated: Date;
  description: string;
  favorite: boolean;
  inactive: boolean;
};

@Injectable({ providedIn: 'root' })
export class AccountAdapter implements Adapter<Account> {
  adapt(item: accountType): Account {
    return new Account(
      item.id,
      item.name,
      item.type,
      item.openBalance,
      item.openDate,
      item.transactionCount,
      item.balance,
      item.updated,
      item.description,
      item.favorite,
      item.inactive
    );
  }
}
