import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { ITransactionDetail } from '@app/shared/interfaces/transaction-detail.interface';
import { IPagingInfo } from '@app/shared/interfaces/paging-info.interface';
import { switchMap, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'dfta-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss']
})
export class TransactionsListComponent implements OnInit {
  @Input() accountId: string;

  transactions$: Observable<IPagingInfo<ITransactionDetail>>;

  page = 1;

  private limit = 10;

  constructor(
    private transactionsService: TransactionsService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.transactions$ = this.route.queryParamMap.pipe(
      switchMap(params => {
        this.page = params.has('page') ? +params.get('page') : 1;
        return this.transactionsService.find(
          this.accountId,
          (this.page - 1) * this.limit,
          this.limit,
          ['-date']
        );
      })
    );
  }

  click(transaction: ITransactionDetail) {
    this.router.navigate(['transactions', transaction.id]/*, { relativeTo: this.route }*/);
  }

  pageChange() {
    this.router.navigate(['./'], { queryParams: { page: this.page }, relativeTo: this.route });
  }
}
