import { Component, OnInit } from '@angular/core';
import { Account } from '@app/core/models';
import { AccountsService } from '@app/core/services';
import { filterNullish } from '@app/shared/helpers';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { TransactionsListComponent } from '@app/shared/components/transactions-list/transactions-list.component';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss'],
  standalone: true,
  imports: [CommonModule, TransactionsListComponent],
})
export class AccountTransactionsComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) {}

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
