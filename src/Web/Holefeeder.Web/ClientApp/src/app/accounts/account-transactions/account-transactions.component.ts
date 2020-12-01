import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'dfta-account-transactions',
  templateUrl: './account-transactions.component.html',
  styleUrls: ['./account-transactions.component.scss']
})
export class AccountTransactionsComponent implements OnInit {
  accountId: string;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.accountId = this.route.parent.snapshot.paramMap.get('accountId');
  }
}
