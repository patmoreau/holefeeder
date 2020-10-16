import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'dfta-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss']
})
export class AccountUpcomingComponent implements OnInit {
  accountId: string;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.accountId = this.route.parent.snapshot.paramMap.get('accountId');
  }
}
