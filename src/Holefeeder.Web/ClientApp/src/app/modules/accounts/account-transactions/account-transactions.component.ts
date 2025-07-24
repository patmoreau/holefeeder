import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { AccountsService } from '@app/core/services';
import { TransactionsListComponent } from '@app/shared/components';
import { filterNullish } from '@app/shared/helpers';
import { Account } from '@app/shared/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss'],
  standalone: true,
  imports: [CommonModule, TransactionsListComponent]
})
export class AccountTransactionsComponent implements OnInit {
  private accountsService = inject(AccountsService);

  account$!: Observable<Account>;

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
