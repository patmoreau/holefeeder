import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { ITransaction, transactionToServer, transactionFromServer } from '../interfaces/transaction.interface';
import { ITransactionDetail } from '../interfaces/transaction-detail.interface';
import { HttpParams } from '@angular/common/http';
import { IPagingInfo } from '../interfaces/paging-info.interface';
import { map, tap } from 'rxjs/operators';
import { UpcomingService } from '@app/singletons/services/upcoming.service';

@Injectable()
export class TransactionsService {
  private basePath = 'api/v1/transactions';

  constructor(private upcomingService: UpcomingService, private api: ApiService) { }

  find(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Promise<IPagingInfo<ITransactionDetail>> {
    let params = new HttpParams();
    if (accountId) {
      params = params.set('filter', `account=${accountId}`);
    }
    if (offset) {
      params = params.set('offset', `${offset}`);
    }
    if (limit) {
      params = params.set('limit', `${limit}`);
    }
    if (sort) {
      sort.forEach(element => {
        params = params.append('sort', `${element}`);
      });
    }
    return this.api
      .getList(this.basePath, params)
      .pipe(
        map(data =>
          Object.assign(
            {},
            {
              totalCount: data.totalCount,
              items: Object.assign(
                [],
                data.items.map(transactionFromServer)
              )
            }
          )
        )
      )
      .toPromise();
  }

  findOneById(id: number | string): Promise<ITransaction> {
    return this.api
      .get(`${this.basePath}/${id}`)
      .pipe(
        map(transactionFromServer)
      )
      .toPromise();
  }

  create(transaction: ITransaction): Promise<ITransaction> {
    return this.api
      .post(`${this.basePath}`, transactionToServer(transaction))
      .pipe(
        map(transactionFromServer),
        tap(_ => this.upcomingService.refreshUpcoming())
      )
      .toPromise();
  }

  update(
    id: number | string,
    transaction: ITransaction
  ): Promise<void> {
    return this.api
      .put(`${this.basePath}/${id}`, transactionToServer(transaction))
      .pipe(
        tap(_ => this.upcomingService.refreshUpcoming())
      ).toPromise();
  }

  delete(id: number | string): Promise<void> {
    return this.api
      .delete(`${this.basePath}/${id}`)
      .pipe(
        tap(_ => this.upcomingService.refreshUpcoming())
      ).toPromise();
  }
}
