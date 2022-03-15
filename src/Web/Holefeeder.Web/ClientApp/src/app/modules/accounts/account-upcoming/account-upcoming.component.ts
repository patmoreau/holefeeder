import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Data} from '@angular/router';
import {map, Observable} from 'rxjs';
import {Account} from "@app/core";

@Component({
  selector: 'app-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private route: ActivatedRoute) {
  }

  ngOnInit() {
    if (this.route.parent) {
      this.account$ = this.route.parent.data.pipe(
        map((data: Data) => data['account'])
      );
    }
  }
}
