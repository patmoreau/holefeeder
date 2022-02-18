import { Component, OnInit } from '@angular/core';
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
  selector: 'app-dashboard-home',
  templateUrl: './dashboard-home.component.html',
  styleUrls: ['./dashboard-home.component.scss']
})
export class DashboardHomeComponent implements OnInit {
  upcoming$!: Observable<Upcoming[]>;

  constructor(private upcomingService: UpcomingService,
    private transactionsService: TransactionsService,
    private adapter: PayCashflowCommandAdapter,
    private messages: MessageService,
    private router: Router) { }

  ngOnInit(): void {
    this.upcoming$ = this.upcomingService.upcoming$;
  }

  async action(event: string, upcoming: Upcoming) {
    if (event === 'EDIT') {
      this.router.navigate(['transactions', 'pay-cashflow', upcoming.id], { queryParams: { date: upcoming.date } });
    } else {
      this.transactionsService.payCashflow(this.adapter.adapt({ date: upcoming.date, amount: upcoming.amount, cashflow: upcoming.id, cashflowDate: upcoming.date }))
        .subscribe(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, { id: id }));
    }
  }
}
