import { Injectable } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import { Account, AccountType, Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class AccountAdapter implements Adapter<Account> {
  adapt(item: {
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
  }): Account {
    return new Account(
      item.id,
      item.name,
      item.type,
      item.openBalance,
      dateFromUtc(item.openDate),
      item.transactionCount,
      item.balance,
      dateFromUtc(item.updated),
      item.description,
      item.favorite,
      item.inactive
    );
  }
}
