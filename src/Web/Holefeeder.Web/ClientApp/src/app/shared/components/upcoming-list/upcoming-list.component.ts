import {Component, Input, OnInit} from '@angular/core';
import {Observable} from 'rxjs';
import {Router} from '@angular/router';
import {MessageService, PayCashflowCommandAdapter, TransactionsService, Upcoming, UpcomingService} from "@app/core";
import {MessageAction, MessageType} from "@app/shared";

@Component({
  selector: 'app-upcoming-list',
  templateUrl: './upcoming-list.component.html',
  styleUrls: ['./upcoming-list.component.scss']
})
export class UpcomingListComponent implements OnInit {
  @Input() accountId: string | undefined;

  upcomingCashflows$: Observable<Upcoming[]> | undefined;

  constructor(
    private upcomingService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter,
    private messages: MessageService,
    private router: Router) {
  }

  ngOnInit() {
    if (this.accountId) {
      this.upcomingCashflows$ = this.upcomingService.getUpcoming(this.accountId);
    }
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      await this.router.navigate(['transactions', 'pay-cashflow', upcoming.id], {queryParams: {date: upcoming.date}});
    } else {
      this.transactionsService.payCashflow(this.adapter.adapt({
        date: upcoming.date,
        amount: upcoming.amount,
        cashflow: upcoming.id,
        cashflowDate: upcoming.date
      }))
        .subscribe(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, {id: id}));
    }
  }
}
