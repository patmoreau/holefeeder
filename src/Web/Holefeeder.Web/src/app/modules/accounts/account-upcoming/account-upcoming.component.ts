import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Account } from '@app/modules/accounts/models/account.model';
import { Observable, switchMap, tap } from 'rxjs';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'dfta-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  account$: Observable<Account> | undefined;

  constructor(
    private route: ActivatedRoute,
    private accountsService: AccountsService) { }

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$;
  }
}
