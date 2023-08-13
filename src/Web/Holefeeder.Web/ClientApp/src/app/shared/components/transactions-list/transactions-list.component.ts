import { CommonModule } from '@angular/common';
import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  Input,
  OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { trace } from '@app/core/logger';
import { TransactionsService } from '@app/core/services';
import { TransactionListItemComponent } from '@app/shared/components';
import { tapTrace } from '@app/shared/helpers';
import { PagingInfo, TransactionDetail } from '@app/shared/models';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss'],
  standalone: true,
  imports: [CommonModule, NgbPaginationModule, TransactionListItemComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class TransactionsListComponent implements OnInit {
  @Input() accountId: string | undefined;

  transactions$: Observable<PagingInfo<TransactionDetail>> | undefined;

  currentPage = 1;
  limit = 15;

  constructor(
    private transactionsService: TransactionsService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.transactions$ = this.route.queryParamMap.pipe(
      filter(params => params !== null),
      tapTrace(),
      switchMap(params => {
        this.currentPage = +(params.get('page') ?? 1);
        return this.accountId
          ? this.transactionsService.find(
              this.accountId,
              (this.currentPage - 1) * this.limit,
              this.limit,
              ['-date']
            )
          : of(new PagingInfo<TransactionDetail>(0, []));
      })
    );
  }

  click(transaction: TransactionDetail) {
    this.router.navigate(['transactions', transaction.id]).then();
  }

  @trace()
  pageChanged(page: number) {
    this.router
      .navigate(['./'], {
        queryParams: { page: page },
        relativeTo: this.route,
      })
      .then();
  }
}
