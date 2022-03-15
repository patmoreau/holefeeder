import {Injectable} from '@angular/core';
import {dateFromUtc} from '@app/shared/date-parser.helper';
import {AccountType} from '@app/shared/enums/account-type.enum';
import {Adapter} from '@app/shared/interfaces/adapter.interface';

export class Account {
  constructor(
    public id: string,
    public name: string,
    public type: AccountType,
    public openBalance: number,
    public openDate: Date,
    public transactionCount: number,
    public balance: number,
    public updated: Date,
    public description: string,
    public favorite: boolean,
    public inactive: boolean
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class AccountAdapter implements Adapter<Account> {
  adapt(item: any): Account {
    return new Account(item.id, item.name, item.type, item.openBalance, dateFromUtc(item.openDate),
      item.transactionCount, item.balance, dateFromUtc(item.updated), item.description, item.favorite, item.inactive);
  }
}