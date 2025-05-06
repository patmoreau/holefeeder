import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AccountsService, UpcomingService } from '@app/core/services';
import { LoaderComponent } from '@app/shared/components';
import { filterNullish } from '@app/shared/helpers';
import { Account, accountTypeMultiplier } from '@app/shared/models';
import { Observable, of, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent]
})
export class AccountDetailsComponent implements OnInit {
  account$!: Observable<Account | undefined>;
  upcomingBalance$!: Observable<number>;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  private static amountClass(amount: number): string {
    if (amount < 0) {
      return 'text-danger';
    } else if (amount > 0) {
      return 'text-success';
    } else {
      return 'text-info';
    }
  }

  ngOnInit() {
    this.account$ = this.route.paramMap.pipe(
      switchMap(params =>
        this.accountsService.findById(params.get('accountId')!)
      ),
      filterNullish(),
      tap(account => this.accountsService.selectAccount(account)),
      switchMap(() => this.accountsService.selectedAccount$),
      filterNullish()
    );

    this.upcomingBalance$ = this.account$.pipe(
      filterNullish(),
      switchMap(account =>
        this.upcomingService
          .getUpcoming(account.id)
          .pipe(
            switchMap(cashflows =>
              of(this.accountsService.getUpcomingBalance(account, cashflows))
            )
          )
      )
    );
  }

  edit() {
    this.router.navigate(['edit'], { relativeTo: this.route }).then();
  }

  addTransaction(account: Account) {
    this.router.navigate(['transactions', 'make-purchase', account.id]).then();
  }

  balanceClass(account: Account): string {
    return AccountDetailsComponent.amountClass(
      account.balance * accountTypeMultiplier(account.type)
    );
  }

  upcomingBalanceClass(account: Account, upcomingBalance: number): string {
    return AccountDetailsComponent.amountClass(
      upcomingBalance * accountTypeMultiplier(account.type)
    );
  }
}
