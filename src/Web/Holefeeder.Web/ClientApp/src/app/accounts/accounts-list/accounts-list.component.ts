import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {AccountsService} from '../../shared/services/accounts.service';
import {IAccountDetail} from '@app/shared/interfaces/account-detail.interface';
import {AccountTypeNames, accountTypeMultiplier} from '@app/shared/enums/account-type.enum';
import {UpcomingService} from '@app/singletons/services/upcoming.service';
import {Subject} from 'rxjs';
import {faPlus} from '@fortawesome/free-solid-svg-icons';
import {IUpcoming} from "@app/shared/interfaces/v2/upcoming.interface";

@Component({
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent implements OnInit {
  accounts: IAccountDetail[];
  upcomingCashflows: IUpcoming[];
  accountTypeNames: Map<string, string>;
  showInactive = false;
  $showInactive = new Subject<boolean>();

  faPlus = faPlus;

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
        showInactive ? 'inactive=true' : 'inactive!=true'
      ]);
    });
    this.upcomingService.cashflows.subscribe(cashflows => this.upcomingCashflows = cashflows);
    this.$showInactive.next(this.showInactive);
  }

  click(id: string) {
    this.router.navigate(['accounts', id]);
  }

  inactiveChange() {
    this.showInactive = !this.showInactive;
    this.$showInactive.next(this.showInactive);
  }

  getUpcomingBalance(account: IAccountDetail): number {
    return (
      account.balance +
      (this.upcomingCashflows ?
        this.upcomingCashflows
          .filter(cashflow => cashflow.account.mongoId === account.id)
          .map(
            cashflow =>
              cashflow.amount *
              cashflow.category.type.multiplier *
              accountTypeMultiplier(account.type)
          )
          .reduce((sum, current) => sum + current, 0) : 0)
    );
  }

  amountClass(account: IAccountDetail): string {
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
