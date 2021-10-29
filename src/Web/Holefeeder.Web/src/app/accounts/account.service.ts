import { Injectable } from "@angular/core";
import { IAccount } from "@app/shared/interfaces/account.interface";
import { BehaviorSubject } from "rxjs";

@Injectable()
export class AccountService {

  // Observable IAccountDetail sources
  private accountSelectedSource = new BehaviorSubject<IAccount>(null);

  // Observable IAccountDetail streams
  accountSelected$ = this.accountSelectedSource.asObservable();

  // Service message commands
  accountSelected(account: IAccount) {
    this.accountSelectedSource.next(account);
  }
}
