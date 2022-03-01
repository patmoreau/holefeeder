import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Data, Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { Upcoming } from "@app/core/models/upcoming.model";
import { categoryTypeMultiplier } from '@app/shared/interfaces/category-type.interface';
import { accountTypeMultiplier } from '@app/shared/interfaces/account-type.interface';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { UpcomingService } from '@app/core/services/upcoming.service';
import { Account } from '@app/core/models/account.model';
import { AccountsService } from '@app/core/services/accounts.service';

@Component({
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
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
    if(this.showInactive) {
      this.accounts$ = this.accountsService.inactiveAccounts$;
    } else{
      this.accounts$ = this.accountsService.activeAccounts$;
    }
  }

  getUpcomingBalance(account: Account, cashflows: Upcoming[]): number {
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
