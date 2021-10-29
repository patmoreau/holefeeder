import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';

@Component({
  selector: 'dfta-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  accountId: string;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.accountService.accountSelected$.subscribe(account => {
      this.accountId = account.id;
    });
  }
}
