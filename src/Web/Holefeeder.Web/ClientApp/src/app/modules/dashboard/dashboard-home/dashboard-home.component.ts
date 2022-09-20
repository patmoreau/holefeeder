import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { PayCashflowCommandAdapter } from '@app/core/adapters';
import {
  MessageService,
  TransactionsService,
  UpcomingService,
} from '@app/core/services';
import {
  LoaderComponent,
  TransactionListItemComponent,
  TransactionsListComponent,
} from '@app/shared/components';
import { MessageAction, MessageType, Upcoming } from '@app/shared/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TransactionsListComponent,
    TransactionListItemComponent,
    LoaderComponent,
  ],
})
export class DashboardHomeComponent implements OnInit {
  upcoming$!: Observable<Upcoming[]>;

  constructor(
    private upcomingService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter,
    private messages: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.upcoming$ = this.upcomingService.upcoming$;
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      await this.router.navigate(
        ['transactions', 'pay-cashflow', upcoming.id],
        { queryParams: { date: upcoming.date } }
      );
    } else {
      this.transactionsService
        .payCashflow(
          this.adapter.adapt({
            date: upcoming.date,
            amount: upcoming.amount,
            cashflow: upcoming.id,
            cashflowDate: upcoming.date,
          })
        )
        .subscribe(id =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          })
        );
    }
  }
}
