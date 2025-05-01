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
import { Observable, of, tap, shareReplay, catchError, map, switchMap, throwError } from 'rxjs';
import { transactionDetailType, TransactionDetailAdapter } from '../adapters';

const apiRoute = 'transactions';
const CACHE_TTL = 60000; // 1 minute cache

type idType = { id: string };

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private cache: Map<string, Observable<PagingInfo<TransactionDetail>>> = new Map();

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: TransactionDetailAdapter,
    private messages: MessageService
  ) { }

  find(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Observable<PagingInfo<TransactionDetail>> {
    const cacheKey = `${accountId}-${offset}-${limit}-${sort.join(',')}`;

    if (this.cache.has(cacheKey)) {
      return this.cache.get(cacheKey)!;
    }

    let params = new HttpParams();
    if (accountId) {
      params = params.set('filter', `AccountId:eq:${accountId}`);
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

    const request = this.http
      .get<transactionDetailType[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(error => {
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load transactions. Please try again later.'
          });
          return throwError(() => error);
        }),
        shareReplay(1)
      );

    this.cache.set(cacheKey, request);

    // Clear cache after TTL
    setTimeout(() => {
      this.cache.delete(cacheKey);
    }, CACHE_TTL);

    return request;
  }

  findById(id: string): Observable<TransactionDetail> {
    return this.http
      .get<transactionDetailType>(`${this.apiUrl}/${apiRoute}/${id}`)
      .pipe(
        map(data => this.adapter.adapt(data)),
        catchError(error => {
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: `Failed to load transaction ${id}. Please try again later.`
          });
          return throwError(() => error);
        })
      );
  }

  payCashflow(transaction: PayCashflowCommand): Observable<string> {
    return this.http
      .post<idType>(`${this.apiUrl}/${apiRoute}/pay-cashflow`, transaction)
      .pipe(
        map(data => data.id),
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
      .post<idType>(`${this.apiUrl}/${apiRoute}/make-purchase`, transaction)
      .pipe(
        map(data => data.id),
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
      .post<idType>(`${this.apiUrl}/${apiRoute}/transfer`, transaction)
      .pipe(
        map(data => data.id),
        catchError(formatErrors),
        tap(id => {
          this.clearCache();
          this.messages.sendMessage({
            type: MessageType.transaction,
            action: MessageAction.post,
            content: { id: id },
          });
        })
      );
  }

  modify(transaction: ModifyTransactionCommand): Observable<void> {
    return this.http.post(`${this.apiUrl}/${apiRoute}/modify`, transaction).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() => {
        this.clearCache();
        this.messages.sendMessage({
          type: MessageType.transaction,
          action: MessageAction.post,
          content: { id: transaction.id },
        });
      })
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete(`${this.apiUrl}/${apiRoute}/${id}`).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() => {
        this.clearCache();
        this.messages.sendMessage({
          type: MessageType.transaction,
          action: MessageAction.delete,
          content: { id: id },
        });
      })
    );
  }

  private clearCache() {
    this.cache.clear();
  }
}
