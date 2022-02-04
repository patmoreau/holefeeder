import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { Upcoming } from "@app/core/models/upcoming.model";
import { PayCashflowCommand } from '@app/shared/transactions/pay-cashflow-command.model';
import { UpcomingService } from '@app/core/services/upcoming.service';

@Component({
  selector: 'dfta-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent implements OnInit {
  upcoming$: Observable<Upcoming[]>;

  constructor(private upcomingService: UpcomingService,
    private cashflowsService: CashflowsService,
    private transactionsService: TransactionsService,
    private router: Router) { }

  ngOnInit(): void {
    this.upcoming$ = this.upcomingService.upcoming$;
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['/transactions/create'], { queryParams: { cashflow: upcoming.id, date: upcoming.date.toISOString() } });
    } else {
      const cashflow = await this.cashflowsService.findOneById(upcoming.id);
      const transaction = new PayCashflowCommand(Object.assign({}, { date: upcoming.date, amount: cashflow.amount, cashflowId: cashflow.id, cashflowDate: upcoming.date }));
      await this.transactionsService.payCashflow(transaction);
      this.upcomingService.refresh();
    }
  }
}
