import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AccountsService, UpcomingService } from '@app/core/services';
import { LoaderComponent } from '@app/shared/components';
import {
  Account,
  accountTypeMultiplier,
  AccountTypeNames,
  Upcoming,
} from '@app/shared/models';
import { Observable } from 'rxjs';

@Component({
    templateUrl: './accounts-list.component.html',
    styleUrls: ['./accounts-list.component.scss'],
    imports: [CommonModule, RouterModule, LoaderComponent]
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
    return this.accountsService.getUpcomingBalance(account, cashflows);
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
