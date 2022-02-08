import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { Upcoming } from "@app/core/models/upcoming.model";
import { UpcomingService } from '@app/core/services/upcoming.service';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { PayCashflowCommand } from '@app/shared/models/pay-cashflow-command.model';

@Component({
  selector: 'dfta-upcoming-list',
  templateUrl: './upcoming-list.component.html',
  styleUrls: ['./upcoming-list.component.scss']
})
export class UpcomingListComponent implements OnInit {
  @Input() accountId: string;

  upcomingCashflows$: Observable<Upcoming[]>;

  constructor(
    private upcomingService: UpcomingService,
    private cashflowsService: CashflowsService,
    private transactionsService: TransactionsService,
    private router: Router) { }

  ngOnInit() {
    this.upcomingCashflows$ = this.upcomingService.getUpcoming(this.accountId);
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['transactions', 'pay-cashflow', upcoming.id, upcoming.date]);
    } else {
      const cashflow = await this.cashflowsService.findOneById(upcoming.id);
      const transaction = new PayCashflowCommand(Object.assign({}, { date: upcoming.date, amount: cashflow.amount, cashflowId: cashflow.id, cashflowDate: upcoming.date }));
      await this.transactionsService.payCashflow(transaction);
      this.upcomingService.refresh();
    }
  }
}
