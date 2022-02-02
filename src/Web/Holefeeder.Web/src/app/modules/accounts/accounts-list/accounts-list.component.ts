import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountsService } from '@app/shared/services/accounts.service';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { Observable, Subject } from 'rxjs';
import { Upcoming } from "@app/core/models/upcoming.model";
import { categoryTypeMultiplier } from '@app/shared/interfaces/category-type.interface';
import { accountTypeMultiplier } from '@app/shared/interfaces/account-type.interface';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { PagingInfo } from '@app/shared/interfaces/paging-info.interface';
import { UpcomingService } from '@app/core/upcoming.service';

@Component({
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent implements OnInit {
  accounts: PagingInfo<IAccount>;
  upcomingCashflows$: Observable<Upcoming[]>;
  accountTypeNames: Map<string, string>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  constructor(
    private accountService: AccountsService,
    private upcomingService: UpcomingService,
    private router: Router
  ) {
    this.accountTypeNames = AccountTypeNames;
  }

  ngOnInit() {
    this.$showInactive.subscribe(async (showInactive) => {
      this.accounts = await this.accountService.find(null, null, [
        '-favorite',
        'name'
      ], [
        showInactive ? 'inactive:eq:true' : 'inactive:eq:false'
      ]);
    });
    this.upcomingCashflows$ = this.upcomingService.upcoming$;
    this.$showInactive.next(this.showInactive);
  }

  click(id: string) {
    this.router.navigate(['accounts', id]);
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    this.$showInactive.next(this.showInactive);
  }

  getUpcomingBalance(account: IAccount, cashflows: Upcoming[]): number {
    return (
      account.balance +
      (cashflows ?
        cashflows
          .filter(cashflow => cashflow.account.id === account.id)
          .map(
            cashflow => {
              return cashflow.amount *
                categoryTypeMultiplier(cashflow.category.type) *
                accountTypeMultiplier(account.type)
            }
          )
          .reduce((sum, current) => sum + current, 0) : 0)
    );
  }

  amountClass(account: IAccount): string {
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
