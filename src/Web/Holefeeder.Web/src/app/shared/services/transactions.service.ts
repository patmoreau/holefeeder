import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { ITransactionDetail, transactionDetailFromServer } from '../interfaces/transaction-detail.interface';
import { HttpParams } from '@angular/common/http';
import { PagingInfo } from '../interfaces/paging-info.interface';
import { map } from 'rxjs/operators';
import { ModifyTransactionCommand } from '../models/modify-transaction-command.model';
import { MakePurchaseCommand } from '../models/make-purchase-command.model';
import { TransferMoneyCommand } from '../models/transfer-money-command.model';
import { PayCashflowCommand } from '../models/pay-cashflow-command.model';

@Injectable()
export class TransactionsService {
  private basePath = 'transactions';

  constructor(private api: ApiService) { }

  find(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Promise<PagingInfo<ITransactionDetail>> {
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
      .find(`${this.api.budgetingBasePath}/${this.basePath}`, params)
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
      .get(`${this.api.budgetingBasePath}/${this.basePath}/${id}`)
      .pipe(
        map(transactionDetailFromServer)
      )
      .toPromise();
  }

  modify(transaction: ModifyTransactionCommand): Promise<void> {
    return this.api.post(`${this.api.budgetingBasePath}/${this.basePath}/modify`, transaction)
         .toPromise();
  }

  makePurchase(transaction: MakePurchaseCommand): Promise<void> {
    return this.api.post(`${this.api.budgetingBasePath}/${this.basePath}/make-purchase`, transaction)
         .toPromise();
  }

  transferMoney(transaction: TransferMoneyCommand): Promise<void> {
    return this.api.post(`${this.api.budgetingBasePath}/${this.basePath}/transfer-money`, transaction)
         .toPromise();
  }

  payCashflow(transaction: PayCashflowCommand): Promise<void> {
    return this.api.post(`${this.api.budgetingBasePath}/${this.basePath}/pay-cashflow`, transaction)
         .toPromise();
  }

  delete(id: number | string): Promise<void> {
    return this.api
      .delete(`${this.api.budgetingBasePath}/${this.basePath}/${id}`)
      .toPromise();
  }
}
