import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Account } from '@app/modules/accounts/models/account.model';
import { filterNullish } from '@app/shared/rxjs.helper';
import { Observable } from 'rxjs';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(
    private route: ActivatedRoute,
    private accountsService: AccountsService) { }

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
