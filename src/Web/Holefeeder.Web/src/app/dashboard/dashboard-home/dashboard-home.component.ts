import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import {IUpcoming} from "@app/shared/interfaces/upcoming.interface";
import { PayCashflowCommand } from '@app/shared/transactions/pay-cashflow-command.model';

@Component({
  selector: 'dfta-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent {
  upcomingCashflows$: Observable<IUpcoming[]>;

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
      const transaction = new PayCashflowCommand(Object.assign({}, {date:upcoming.date, amount:cashflow.amount, cashflowId:cashflow.id, cashflowDate:upcoming.date}));
      await this.transactionsService.payCashflow(transaction);
      await this.upcomingService.refreshUpcoming();
    }
  }
}
