import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {AccountsService} from '@app/shared/services/accounts.service';
import {IAccountDetail} from '@app/shared/interfaces/account-detail.interface';
import {UpcomingService} from '@app/singletons/services/upcoming.service';
import {accountTypeMultiplier} from '@app/shared/enums/account-type.enum';
import {faArrowLeft, faEdit, faPlus} from '@fortawesome/free-solid-svg-icons';
import {IUpcoming} from "@app/shared/interfaces/v2/upcoming.interface";

@Component({
  selector: 'dfta-account-details',
  templateUrl: './account-details.component.html',
  styleUrls: ['./account-details.component.scss']
})
export class AccountDetailsComponent implements OnInit {
  account: IAccountDetail;
  upcomingCashflows: IUpcoming[];
  upcomingBalance = 0;
  isLoaded = false;
  faArrowLeft = faArrowLeft;
  faEdit = faEdit;
  faPlus = faPlus;

  constructor(
    private accountsService: AccountsService,
    private upcomingService: UpcomingService,
    private router: Router,
    private route: ActivatedRoute
  ) {
  }

  async ngOnInit() {
    if (this.route.snapshot.paramMap.has('accountId')) {
      this.account = await this.accountsService.findOneByIdWithDetails(
        this.route.snapshot.paramMap.get('accountId')
      );
    } else {
      this.router.navigate(['./']);
    }

    this.upcomingService.cashflows.subscribe(cashflows =>
      this.upcomingBalance =
        this.account.balance +
        cashflows
          .filter(cashflow => cashflow.account.mongoId === this.account.id)
          .map(cashflow =>
            cashflow.amount *
            cashflow.category.type.multiplier *
            accountTypeMultiplier(this.account.type)
          )
          .reduce((sum, current) => sum + current, 0)
    );

    this.isLoaded = true;
  }

  edit() {
    this.router.navigate(['edit'], {relativeTo: this.route});
  }

  addTransaction() {
    this.router.navigate(['transactions', 'create'], {queryParams: {accountId: this.account.id}});
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
