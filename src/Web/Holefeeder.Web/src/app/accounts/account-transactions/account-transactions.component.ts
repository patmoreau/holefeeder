import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';

@Component({
  selector: 'dfta-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  accountId: string;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.accountService.accountSelected$.subscribe(account => {
      this.accountId = account.id;
    });
  }
}
