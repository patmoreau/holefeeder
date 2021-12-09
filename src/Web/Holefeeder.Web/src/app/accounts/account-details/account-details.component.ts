import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { IUpcoming } from "@app/shared/interfaces/upcoming.interface";
import { categoryTypeMultiplier } from '@app/shared/interfaces/category-type.interface';
import { AccountsService } from '@app/shared/services/accounts.service';
import { accountTypeMultiplier } from '@app/shared/interfaces/account-type.interface';
import { AccountService } from '../account.service';

@Component({
  selector: 'dfta-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss'],
  providers: [AccountService]
})
export class AccountDetailsComponent implements OnInit {
  account: IAccount;
  upcomingCashflows: IUpcoming[];
  upcomingBalance = 0;
  isLoaded = false;

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
      const accountId = params['accountId'];
      this.account = await this.accountsService.findOneByIdWithDetails(accountId);
      this.accountService.accountSelected(this.account);

      this.isLoaded = true;
      this.upcomingService.cashflows.subscribe(cashflows =>
        this.upcomingBalance =
        this.account.balance +
        cashflows
          .filter(cashflow => cashflow.account.id === this.account.id)
          .map(cashflow =>
            cashflow.amount *
            categoryTypeMultiplier(cashflow.category.type) *
            accountTypeMultiplier(this.account.type)
          )
          .reduce((sum, current) => sum + current, 0)
      );
      });

  }

  edit() {
    this.router.navigate(['edit'], { relativeTo: this.route });
  }

  addTransaction() {
    this.router.navigate(['transactions', 'create'], { queryParams: { accountId: this.account.id } });
  }

  balanceClass(): string {
    return this.amountClass(this.account.balance * accountTypeMultiplier(this.account.type));
  }

  upcomingBalanceClass(): string {
    return this.amountClass(this.upcomingBalance * accountTypeMultiplier(this.account.type));
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
