import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Account, Upcoming } from '@app/core/models';
import { AccountsService, UpcomingService } from '@app/core/services';
import {
  accountTypeMultiplier,
  AccountTypeNames,
  categoryTypeMultiplier,
} from '@app/shared/models';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from '@app/shared';

@Component({
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
})
export class AccountsListComponent implements OnInit {
  accounts$!: Observable<Account[]>;
  upcomingCashflows$: Observable<Upcoming[]>;
  accountTypeNames: Map<string, string>;
  showInactive = false;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.accountTypeNames = AccountTypeNames;

    this.upcomingCashflows$ = this.upcomingService.upcoming$;
  }

  ngOnInit() {
    this.accounts$ = this.accountsService.activeAccounts$;
  }

  click(account: Account) {
    this.router.navigate(['accounts', account.id]);
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    if (this.showInactive) {
      this.accounts$ = this.accountsService.inactiveAccounts$;
    } else {
      this.accounts$ = this.accountsService.activeAccounts$;
    }
  }

  getUpcomingBalance(account: Account, cashflows: Upcoming[]): number {
    return (
      account.balance +
      (cashflows
        ? cashflows
            .filter(cashflow => cashflow.account.id === account.id)
            .map(cashflow => {
              return (
                cashflow.amount *
                categoryTypeMultiplier(cashflow.category.type) *
                accountTypeMultiplier(account.type)
              );
            })
            .reduce((sum, current) => sum + current, 0)
        : 0)
    );
  }

  amountClass(account: Account): string {
    const val = account.balance * accountTypeMultiplier(account.type);
    if (val < 0) {
      return 'text-danger';
    } else if (val > 0) {
      return 'text-success';
    } else {
      return 'text-info';
    }
  }
}
