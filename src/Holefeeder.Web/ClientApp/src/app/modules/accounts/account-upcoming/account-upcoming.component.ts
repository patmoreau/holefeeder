import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AccountsService } from '@app/core/services';
import { UpcomingListComponent } from '@app/shared/components';
import { filterNullish } from '@app/shared/helpers';
import { Account } from '@app/shared/models';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-account-upcoming',
    templateUrl: './account-upcoming.component.html',
    styleUrls: ['./account-upcoming.component.scss'],
    imports: [CommonModule, UpcomingListComponent]
})
export class AccountUpcomingComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) {}

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
