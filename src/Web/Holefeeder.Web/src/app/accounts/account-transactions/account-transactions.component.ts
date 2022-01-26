import { Component, OnInit } from '@angular/core';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { Observable } from 'rxjs';
import { AccountService } from '../account.service';

@Component({
  selector: 'dfta-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  account$: Observable<IAccount> | undefined;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.account$ = this.accountService.accountSelected$;
  }
}
