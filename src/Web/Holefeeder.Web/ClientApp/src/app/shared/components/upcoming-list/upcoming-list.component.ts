import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { Router, ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { Transaction } from '@app/shared/models/transaction.model';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import {IUpcoming} from "@app/shared/interfaces/v2/upcoming.interface";

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
      map(upcomingCashflows => upcomingCashflows.filter(upcomingCashflow => upcomingCashflow.account.mongoId === this.accountId)),
    );
  }

  async action(event: string, upcoming: IUpcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['transactions', 'create'],
        { /*relativeTo: this.route,*/ queryParams: { cashflow: upcoming.id, date: upcoming.date } });
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
