import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data} from '@angular/router';
import {map, Observable} from 'rxjs';
import {Account, AccountsService} from "@app/core";
import {filterNullish} from "@app/shared";

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) {
  }

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(
      filterNullish()
    );
  }
}
