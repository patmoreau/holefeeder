import { Component, OnInit } from '@angular/core';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { Observable } from 'rxjs';
import { AccountService } from '../account.service';

@Component({
  selector: 'dfta-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  account$: Observable<IAccount> | undefined;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.account$ = this.accountService.accountSelected$;
  }
}
