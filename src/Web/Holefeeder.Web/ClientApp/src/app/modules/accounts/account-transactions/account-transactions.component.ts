import { Component, OnInit } from '@angular/core';
import { filterNullish } from '@app/shared/rxjs.helper';
import { Observable } from 'rxjs';
import { Account } from '../models/account.model';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) { }

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
