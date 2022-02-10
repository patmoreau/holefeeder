import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { Upcoming } from "@app/core/models/upcoming.model";
import { UpcomingService } from '@app/core/services/upcoming.service';
import { TransactionsService } from '@app/core/services/transactions.service';
import { PayCashflowCommandAdapter } from '@app/core/models/pay-cashflow-command.model';
import { MessageService } from '@app/core/services/message.service';
import { MessageType } from '@app/shared/enums/message-type.enum';
import { MessageAction } from '@app/shared/enums/message-action.enum';

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
    private router: Router) { }

  ngOnInit() {
    if (this.accountId) {
      this.upcomingCashflows$ = this.upcomingService.getUpcoming(this.accountId);
    }
  }

  action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['transactions', 'pay-cashflow', upcoming.id, upcoming.date]);
    } else {
      this.transactionsService.payCashflow(this.adapter.adapt({ date: upcoming.date, amount: upcoming.amount, cashflow: upcoming.id, cashflowDate: upcoming.date }))
        .subscribe(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, { id: id }));
    }
  }
}
