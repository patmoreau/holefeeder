import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data} from '@angular/router';
import {map, Observable} from 'rxjs';
import {Account, AccountsService} from "@app/core";
import {filterNullish} from "@app/shared";

@Component({
  selector: 'app-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) {
  }

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(
      filterNullish()
    );
  }
}
