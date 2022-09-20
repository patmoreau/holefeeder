import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PayCashflowCommandAdapter, Upcoming } from '@app/core/models';
import {
  MessageService,
  TransactionsService,
  UpcomingService,
} from '@app/core/services';
import { MessageAction, MessageType } from '@app/shared/models';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { TransactionListItemComponent } from '@app/shared';

@Component({
  selector: 'app-upcoming-list',
  templateUrl: './upcoming-list.component.html',
  styleUrls: ['./upcoming-list.component.scss'],
  standalone: true,
  imports: [CommonModule, NgbPaginationModule, TransactionListItemComponent],
})
export class UpcomingListComponent implements OnInit {
  @Input() accountId: string | undefined;

  upcomingCashflows$: Observable<Upcoming[]> | undefined;

  constructor(
    private upcomingService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter,
    private messages: MessageService,
    private router: Router
  ) {}

  ngOnInit() {
    if (this.accountId) {
      this.upcomingCashflows$ = this.upcomingService.getUpcoming(
        this.accountId
      );
    }
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
