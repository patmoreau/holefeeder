import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ConfigService } from '@app/core/config/config.service';
import { PagingInfo } from '@app/core/models/paging-info.model';
import { Observable, of } from 'rxjs';
import { BaseApiService } from './base-api.service';
import { TransactionDetail, TransactionDetailAdapter } from '@app/core/models/transaction-detail.model';
import { ModifyTransactionCommand } from '@app/core/models/modify-transaction-command.model';
import { MakePurchaseCommand } from '@app/core/models/make-purchase-command.model';
import { TransferMoneyCommand } from '@app/core/models/transfer-money-command.model';
import { PayCashflowCommand } from '@app/core/models/pay-cashflow-command.model';

const apiRoute: string = 'budgeting/api/v2/transactions';

@Injectable({ providedIn: 'root' })
export class TransactionsApiService extends BaseApiService {
  private basePath = 'transactions';

  constructor(private http: HttpClient, private configService: ConfigService, private adapter: TransactionDetailAdapter) {
    super();
  }

  find(accountId: string, offset: number, limit: number, sort: string[]): Observable<PagingInfo<TransactionDetail>> {
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
    return this.http
      .get<Object[]>(`${this.configService.config.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params
      }).pipe(
        map(resp => this.mapToPagingInfo(resp, this.adapter)),
        catchError(this.formatErrors)
      );
  }

  findOneById(id: number | string): Observable<TransactionDetail> {
    return this.http
      .get(`${this.configService.config.apiUrl}/${apiRoute}/${id}`)
      .pipe(
        map(this.adapter.adapt),
        catchError(this.formatErrors)
      );
  }

  modify(transaction: ModifyTransactionCommand): Observable<void> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/modify`, transaction)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(this.formatErrors)
      );
  }

  makePurchase(transaction: MakePurchaseCommand): Observable<string> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/make-purchase`, transaction)
      .pipe(
        map((data: any) => data.id),
        catchError(this.formatErrors)
      );
  }

  transferMoney(transaction: TransferMoneyCommand): Observable<void> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/transfer-money`, transaction)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(this.formatErrors)
      );
  }

  payCashflow(transaction: PayCashflowCommand): Observable<string> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/pay-cashflow`, transaction)
      .pipe(
        map((data: any) => data.id),
        catchError(this.formatErrors)
      );
  }

  delete(id: string): Observable<void> {
    return this.http
      .delete(`${this.configService.config.apiUrl}/${apiRoute}/${id}`)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(this.formatErrors)
      );
  }
}
