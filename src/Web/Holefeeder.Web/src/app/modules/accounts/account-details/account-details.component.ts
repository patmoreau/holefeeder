import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { categoryTypeMultiplier } from '@app/shared/interfaces/category-type.interface';
import { accountTypeMultiplier } from '@app/shared/interfaces/account-type.interface';
import { AccountsService } from '../services/accounts.service';
import { filter, from, Observable, switchMap, of, scan, tap, map, switchMapTo } from 'rxjs';
import { Account } from '../models/account.model';
import { UpcomingService } from '@app/core/services/upcoming.service';

@Component({
  selector: 'dfta-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss']
})
export class AccountDetailsComponent implements OnInit {
  account$: Observable<Account> | undefined;
  upcomingBalance$: Observable<number> | undefined;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private router: Router,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.account$ = this.route.params
      .pipe(
        switchMap(params => this.accountsService.findById(params['accountId'])),
        tap((account: Account) => this.accountsService.selectAccount(account))
      );

    this.upcomingBalance$ = this.account$.pipe(
      filter(account => account !== undefined),
      switchMap(account => this.upcomingService.getUpcoming(account.id)
        .pipe(
          switchMap(cashflows => from(cashflows)),
          scan((sum, cashflow) => sum + (cashflow.amount *
            categoryTypeMultiplier(cashflow.category.type) * accountTypeMultiplier(account.type)), account.balance)
        ))
    );
  }

  edit() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }

  addTransaction(account: Account) {
    this.router.navigate(['transactions', 'create'], { queryParams: { accountId: account.id } });
  }

  balanceClass(account: Account): string {
    return this.amountClass(account.balance * accountTypeMultiplier(account.type));
  }

  upcomingBalanceClass(account: Account, upcomingBalance: number): string {
    return this.amountClass(upcomingBalance * accountTypeMultiplier(account.type));
  }

  private amountClass(amount: number): string {
    if (amount < 0) {
      return 'text-danger';
    } else if (amount > 0) {
      return 'text-success';
    } else {
      return 'text-info';
    }
  }
}
