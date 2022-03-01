import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data} from '@angular/router';
import { Account } from '@app/core/models/account.model';
import {map, Observable} from 'rxjs';

@Component({
  selector: 'app-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    if (this.route.parent) {
      this.account$ = this.route.parent.data.pipe(
        map((data: Data) => data['account']),
      );
    }
  }
}
