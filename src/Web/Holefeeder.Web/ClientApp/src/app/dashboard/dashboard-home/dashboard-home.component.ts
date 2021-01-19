import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { Transaction } from '@app/shared/models/transaction.model';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import {IUpcoming} from "@app/shared/interfaces/v2/upcoming.interface";

@Component({
  selector: 'dfta-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent {
  upcomingCashflows$: Observable<IUpcoming[]>;

  faPlus = faPlus;

  constructor(private upcomingService: UpcomingService,
    private cashflowsService: CashflowsService,
    private transactionsService: TransactionsService,
    private router: Router,
    private route: ActivatedRoute) {
    this.upcomingCashflows$ = this.upcomingService.cashflows;
  }

  async action(event: string, upcoming: IUpcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['/transactions/create'], { queryParams: { cashflow: upcoming.id, date: upcoming.date.toISOString() } });
    } else {
      const cashflow = await this.cashflowsService.findOneById(upcoming.id);
      const transaction = Object.assign(new Transaction(), {
        account: cashflow.account,
        cashflow: cashflow.id,
        category: cashflow.category,
        amount: cashflow.amount,
        id: undefined,
        date: upcoming.date,
        cashflowDate: upcoming.date,
        description: cashflow.description,
        tags: cashflow.tags ? cashflow.tags : []
      });
      this.transactionsService.create(transaction);
    }
  }
}
