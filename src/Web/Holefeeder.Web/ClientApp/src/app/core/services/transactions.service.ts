import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { MessageService } from '@app/core/services';
import { formatErrors, mapToPagingInfo } from '@app/core/utils/api.utils';
import {
  MakePurchaseCommand,
  MessageAction,
  MessageType,
  ModifyTransactionCommand,
  PagingInfo,
  PayCashflowCommand,
  TransactionDetail,
  TransferMoneyCommand,
} from '@app/shared/models';
import { Observable, of, tap } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { TransactionDetailAdapter } from '../adapters';

const apiRoute: string = 'api/v2/transactions';

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: TransactionDetailAdapter,
    private messages: MessageService
  ) {}

  find(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Observable<PagingInfo<TransactionDetail>> {
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
      .get<Object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(formatErrors)
      );
  }

  findById(id: string): Observable<TransactionDetail> {
    return this.http
      .get(`${this.apiUrl}/${apiRoute}/${id}`)
      .pipe(map(this.adapter.adapt), catchError(formatErrors));
  }

  payCashflow(transaction: PayCashflowCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/pay-cashflow`, transaction)
      .pipe(
        map((data: any) => data.id),
        catchError(formatErrors),
        tap(id =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          })
        )
      );
  }

  makePurchase(transaction: MakePurchaseCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/make-purchase`, transaction)
      .pipe(
        map((data: any) => data.id),
        catchError(formatErrors),
        tap(id =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          })
        )
      );
  }

  transfer(transaction: TransferMoneyCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/transfer`, transaction)
      .pipe(
        map((data: any) => data.id),
        catchError(formatErrors),
        tap(id =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          })
        )
      );
  }

  modify(transaction: ModifyTransactionCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/modify`, transaction)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(formatErrors),
        tap(_ =>
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: transaction.id },
          })
        )
      );
  }

  delete(id: string): Observable<void> {
    return this.http.delete(`${this.apiUrl}/${apiRoute}/${id}`).pipe(
      switchMap(_ => of(void 0)),
      catchError(formatErrors),
      tap(_ =>
        this.messages.sendMessage({
          type: MessageType.transaction,
          action: MessageAction.delete,
          content: { id: id },
        })
      )
    );
  }
}
