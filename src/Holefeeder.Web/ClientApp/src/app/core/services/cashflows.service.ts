import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnDestroy, inject } from '@angular/core';
import { MessageService } from '@app/core/services';
import { formatErrors, mapToPagingInfo } from '@app/core/utils/api.utils';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';
import {
  CashflowDetail,
  MessageAction,
  MessageType,
  ModifyCashflowCommand,
  PagingInfo,
  ICategoryInfo,
  IAccountInfo,
  DateIntervalType,
} from '@app/shared/models';
import { Observable, of, tap, catchError, map, switchMap, throwError, shareReplay, filter, debounceTime, takeUntil, Subject, timer } from 'rxjs';
import { CashflowDetailAdapter } from '../adapters';
import { StateService } from './state.service';

const apiRoute = 'cashflows';
const CACHE_TTL = 300000; // 5 minutes cache
const DEBOUNCE_TIME = 300; // 300ms debounce

interface CashflowState {
  cashflows: CashflowDetail[];
  selected: CashflowDetail | null;
  lastUpdate: number;
}

const initialState: CashflowState = {
  cashflows: [],
  selected: null,
  lastUpdate: 0
};

@Injectable({ providedIn: 'root' })
export class CashflowsService extends StateService<CashflowState> implements OnDestroy {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);
  private adapter = inject(CashflowDetailAdapter);
  private messages = inject(MessageService);

  private cashflowsCache: Map<string, Observable<PagingInfo<CashflowDetail>>> = new Map();
  private readonly destroy$ = new Subject<void>();

  inactiveCashflows$: Observable<CashflowDetail[]> = this.select(state =>
    state.cashflows.filter(x => x.inactive)
  );

  activeCashflows$: Observable<CashflowDetail[]> = this.select(state =>
    state.cashflows.filter(x => !x.inactive)
  );

  constructor() {
    super(initialState);
    this.initializeSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    super.destroy();
    this.clearCache();
  }

  private initializeSubscriptions(): void {
    // Listen for cashflow changes and reload data
    this.messages.listen
      .pipe(
        filter(message => message.type === MessageType.cashflow),
        debounceTime(DEBOUNCE_TIME),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.clearCache();
        this.load();
      });

    // Initial load
    this.load();
  }

  find(
    offset: number,
    limit: number,
    sort: string[]
  ): Observable<PagingInfo<CashflowDetail>> {
    const cacheKey = `${offset}-${limit}-${sort.join(',')}`;

    if (this.cashflowsCache.has(cacheKey)) {
      return this.cashflowsCache.get(cacheKey)!;
    }

    let params = new HttpParams();
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
      .get<object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(error => {
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load cashflows. Please try again later.'
          });
          return throwError(() => error);
        }),
        shareReplay(1)
      );

    this.cashflowsCache.set(cacheKey, request);

    // Clear cache after TTL using observable timer instead of setTimeout
    timer(CACHE_TTL)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.clearCacheEntry(cacheKey);
      });

    return request;
  }

  findById(id: string): Observable<CashflowDetail | undefined> {
    if (!this.state.lastUpdate || Date.now() - this.state.lastUpdate > CACHE_TTL) {
      return this.http
        .get<{
          id: string;
          effectiveDate: Date;
          amount: number;
          intervalType: DateIntervalType;
          frequency: number;
          recurrence: number;
          description: string;
          category: ICategoryInfo;
          account: IAccountInfo;
          inactive: boolean;
          tags: string[];
        }>(`${this.apiUrl}/${apiRoute}/${id}`)
        .pipe(
          map(data => this.adapter.adapt(data)),
          catchError(error => {
            this.messages.sendMessage({
              type: MessageType.error,
              action: MessageAction.error,
              content: `Failed to load cashflow ${id}. Please try again later.`
            });
            return throwError(() => error);
          })
        );
    }

    return this.select(state =>
      state.cashflows.find(cashflow => cashflow.id === id)
    );
  }

  modify(cashflow: ModifyCashflowCommand): Observable<void> {
    return this.http.post(`${this.apiUrl}/${apiRoute}/modify`, cashflow).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() => {
        this.clearCache();
        this.messages.sendMessage({
          type: MessageType.cashflow,
          action: MessageAction.post,
          content: { id: cashflow.id },
        });
      })
    );
  }

  cancel(id: string): Observable<void> {
    return this.http.post(`${this.apiUrl}/${apiRoute}/cancel`, { id: id }).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() => {
        this.clearCache();
        this.messages.sendMessage({
          type: MessageType.cashflow,
          action: MessageAction.post,
          content: { id: id },
        });
      })
    );
  }

  private load() {
    this.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: pagingInfo =>
          this.setState({
            cashflows: pagingInfo.items,
            selected: this.state.selected,
            lastUpdate: Date.now()
          }),
        error: error => {
          console.error('Failed to load cashflows in private load method:', error);
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load cashflows. Please try again later.'
          });
        }
      });
  }

  private getAll(): Observable<PagingInfo<CashflowDetail>> {
    const params = new HttpParams().append('sort', 'description');

    return this.http
      .get<object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(error => {
          console.error('HTTP error in getAll cashflows:', error);
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load cashflows. Please try again later.'
          });
          return throwError(() => error);
        }),
        shareReplay(1)
      );
  }

  private clearCache() {
    this.cashflowsCache.clear();
  }

  private clearCacheEntry(cacheKey: string) {
    if (this.cashflowsCache.has(cacheKey)) {
      this.cashflowsCache.delete(cacheKey);
    }
  }
}
