import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';
import { dateFromUtc } from '@app/shared/helpers';
import { Account } from '@app/core/models/account.model';

@Injectable({ providedIn: 'root' })
export class AccountAdapter implements Adapter<Account> {
  adapt(item: any): Account {
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
