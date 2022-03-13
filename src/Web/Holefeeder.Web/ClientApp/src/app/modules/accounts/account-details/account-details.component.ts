import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data, Router} from '@angular/router';
import {categoryTypeMultiplier} from '@app/shared/interfaces/category-type.interface';
import {accountTypeMultiplier} from '@app/shared/interfaces/account-type.interface';
import {filter, from, Observable, switchMap, scan, map} from 'rxjs';
import {Account, AccountsService, UpcomingService} from "@app/core";

@Component({
  selector: 'app-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss']
})
export class AccountDetailsComponent implements OnInit {
  account$!: Observable<Account | undefined>;
  upcomingBalance$!: Observable<number>;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private router: Router,
    private route: ActivatedRoute
  ) {
  }

  ngOnInit() {
    this.account$ = this.route.data.pipe(
      map((data: Data) => data['account']),
    );

    this.upcomingBalance$ = this.account$.pipe(
      filter(account => account !== undefined),
      switchMap(account => this.upcomingService.getUpcoming(account!.id)
        .pipe(
          switchMap(cashflows => from(cashflows)),
          scan((sum, cashflow) => sum + (cashflow.amount *
            categoryTypeMultiplier(cashflow.category.type) * accountTypeMultiplier(account!.type)), account!.balance)
        ))
    );
  }

  edit() {
    this.router.navigate(['edit'], {relativeTo: this.route});
  }

  addTransaction(account: Account) {
    this.router.navigate(['transactions', 'make-purchase', account.id]);
  }

  balanceClass(account: Account): string {
    return AccountDetailsComponent.amountClass(account.balance * accountTypeMultiplier(account.type));
  }

  upcomingBalanceClass(account: Account, upcomingBalance: number): string {
    return AccountDetailsComponent.amountClass(upcomingBalance * accountTypeMultiplier(account.type));
  }

  private static amountClass(amount: number): string {
    if (amount < 0) {
      return 'text-danger';
    } else if (amount > 0) {
      return 'text-success';
    } else {
      return 'text-info';
    }
  }
}
