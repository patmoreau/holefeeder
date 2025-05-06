import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PayCashflowCommandAdapter } from '@app/core/adapters';
import {
  MessageService,
  TransactionsService,
  UpcomingService,
} from '@app/core/services';
import { TransactionListItemComponent } from '@app/shared/components';
import { MessageAction, MessageType, Upcoming } from '@app/shared/models';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-upcoming-list',
  standalone: true,
  templateUrl: './upcoming-list.component.html',
  styleUrls: ['./upcoming-list.component.scss'],
  imports: [CommonModule, NgbPaginationModule, TransactionListItemComponent]
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
  ) { }

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
