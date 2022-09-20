import { Injectable } from '@angular/core';
import { Account } from '@app/core/models/account.model';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

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
