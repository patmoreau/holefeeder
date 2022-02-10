import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { TransactionsService } from '@app/core/services/transactions.service';
import { TransactionDetail } from '@app/core/models/transaction-detail.model';
import { PagingInfo } from '@app/core/models/paging-info.model';

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss']
})
export class TransactionsListComponent implements OnInit {
  @Input() accountId: string | undefined;

  transactions$: Observable<PagingInfo<TransactionDetail>> | undefined;

  page = 1;

  private limit = 10;

  constructor(
    private transactionsService: TransactionsService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.transactions$ = this.route.queryParamMap.pipe(
      filter(params => params !== null),
      switchMap(params => {
        this.page = +(params.get('page') ?? 1);
        return this.accountId ?
          this.transactionsService.find(
            this.accountId,
            (this.page - 1) * this.limit,
            this.limit,
            ['-date']) :
          of(new PagingInfo<TransactionDetail>(0, []));
      })
    );
  }

  click(transaction: TransactionDetail) {
    this.router.navigate(['transactions', transaction.id]);
  }

  pageChange() {
    this.router.navigate(['./'], { queryParams: { page: this.page }, relativeTo: this.route });
  }
}