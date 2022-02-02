import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { categoryTypeMultiplier } from '@app/shared/interfaces/category-type.interface';
import { AccountsService } from '@app/shared/services/accounts.service';
import { accountTypeMultiplier } from '@app/shared/interfaces/account-type.interface';
import { AccountService } from '../account.service';
import { filter, from, Observable, tap, switchMap, of, scan } from 'rxjs';
import { UpcomingService } from '@app/core/upcoming.service';

@Component({
  selector: 'dfta-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss'],
  providers: [AccountService]
})
export class AccountDetailsComponent implements OnInit {
  account$: Observable<IAccount> | undefined;
  upcomingBalance$: Observable<number> | undefined;

  account: IAccount | undefined;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.route.params.subscribe(async params => {
      this.account$ = this.accountsService.findOneByIdWithDetails(params['accountId'])
        .pipe(
          tap(account => this.accountService.accountSelected(account))
        );

      this.upcomingBalance$ = this.account$.pipe(
        switchMap(account => {
          const accountMutiplier = accountTypeMultiplier(account.type);
          return account ? this.upcomingService.upcoming$
            .pipe(
              switchMap(cashflows => from(cashflows)),
              filter(cashflow => cashflow.account.id === account.id),
              scan((sum, cashflow) => sum + (cashflow.amount *
                categoryTypeMultiplier(cashflow.category.type) * accountMutiplier), account.balance)
            ) : of(0);
        })
      )
    });
  }

  edit() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }

  addTransaction(account: IAccount) {
    this.router.navigate(['transactions', 'create'], { queryParams: { accountId: account.id } });
  }

  balanceClass(account: IAccount): string {
    return this.amountClass(account.balance * accountTypeMultiplier(account.type));
  }

  upcomingBalanceClass(account: IAccount, upcomingBalance: number): string {
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
