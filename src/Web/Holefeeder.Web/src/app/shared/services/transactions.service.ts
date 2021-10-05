import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { ITransactionDetail, transactionDetailFromServer } from '../interfaces/transaction-detail.interface';
import { HttpParams } from '@angular/common/http';
import { IPagingInfo } from '../interfaces/paging-info.interface';
import { map } from 'rxjs/operators';
import { UpcomingService } from '@app/singletons/services/upcoming.service';
import { MakePurchaseCommand } from '../transactions/make-purchase-command.model';
import { TransferMoneyCommand } from '../transactions/transfer-money-command.model';

@Injectable()
export class TransactionsService {
  private basePath = 'budgeting/api/v2/transactions';

  constructor(private upcomingService: UpcomingService, private api: ApiService) { }

  find(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Promise<IPagingInfo<ITransactionDetail>> {
    let params = new HttpParams();
    if (accountId) {
      params = params.set('filter', `account_id:eq:${accountId}`);
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
      .get(`${this.basePath}/get-transactions`, params)
      .pipe(
        map(data =>
          Object.assign(
            {},
            {
              totalCount: data.totalCount,
              items: Object.assign(
                [],
                data.items.map(transactionDetailFromServer)
              )
            }
          )
        )
      )
      .toPromise();
  }

  findOneById(id: number | string): Promise<ITransactionDetail> {
    return this.api
      .get(`${this.basePath}/${id}`)
      .pipe(
        map(transactionDetailFromServer)
      )
      .toPromise();
  }

  makePurchase(transaction: MakePurchaseCommand): Promise<void> {
    return this.api.post(`${this.basePath}/make-purchase`, transaction)
         .toPromise();
  }

  transferMoney(transaction: TransferMoneyCommand): Promise<void> {
    return this.api.post(`${this.basePath}/transfer-money`, transaction)
         .toPromise();
  }
}
