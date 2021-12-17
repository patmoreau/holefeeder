import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { Router, ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { IUpcoming } from "@app/shared/interfaces/upcoming.interface";
import { PayCashflowCommand } from '@app/shared/transactions/pay-cashflow-command.model';

@Component({
  selector: 'dfta-upcoming-list',
  templateUrl: './upcoming-list.component.html',
  styleUrls: ['./upcoming-list.component.scss']
})
export class UpcomingListComponent implements OnInit {
  @Input() accountId: string;

  upcomingCashflows$: Observable<IUpcoming[]>;

  constructor(
    private upcomingService: UpcomingService,
    private cashflowsService: CashflowsService,
    private transactionsService: TransactionsService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.upcomingCashflows$ = this.upcomingService.cashflows.pipe(
      map(upcomingCashflows => upcomingCashflows.filter(upcomingCashflow => upcomingCashflow.account.id === this.accountId)),
    );
  }

  async action(event: string, upcoming: IUpcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['transactions', 'create'],
        { /*relativeTo: this.route,*/ queryParams: { cashflow: upcoming.id, date: upcoming.date } });
    } else {
      const cashflow = await this.cashflowsService.findOneById(upcoming.id);
      const transaction = new PayCashflowCommand(Object.assign({}, { date: upcoming.date, amount: cashflow.amount, cashflowId: cashflow.id, cashflowDate: upcoming.date }));
      await this.transactionsService.payCashflow(transaction);
      await this.upcomingService.refreshUpcoming();
    }
  }
}
