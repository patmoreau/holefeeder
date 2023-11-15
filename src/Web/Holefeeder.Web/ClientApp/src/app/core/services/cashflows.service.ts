import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { MessageService } from '@app/core/services';
import { formatErrors, mapToPagingInfo } from '@app/core/utils/api.utils';
import {
  CashflowDetail,
  MessageAction,
  MessageType,
  ModifyCashflowCommand,
  PagingInfo,
} from '@app/shared/models';
import { filter, Observable, of, tap } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { CashflowDetailAdapter } from '../adapters';
import { StateService } from './state.service';

const apiRoute = 'api/v2/cashflows';

interface CashflowState {
  cashflows: CashflowDetail[];
  selected: CashflowDetail | null;
}

const initialState: CashflowState = {
  cashflows: [],
  selected: null,
};

@Injectable({ providedIn: 'root' })
export class CashflowsService extends StateService<CashflowState> {
  inactiveCashflows$: Observable<CashflowDetail[]> = this.select(state =>
    state.cashflows.filter(x => x.inactive)
  );
  activeCashflows$: Observable<CashflowDetail[]> = this.select(state =>
    state.cashflows.filter(x => !x.inactive)
  );

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: CashflowDetailAdapter,
    private messages: MessageService
  ) {
    super(initialState);

    this.messages.listen
      .pipe(filter(message => message.type === MessageType.cashflow))
      .subscribe(() => {
        this.load();
      });

    this.load();
  }

  find(
    offset: number,
    limit: number,
    sort: string[]
  ): Observable<PagingInfo<CashflowDetail>> {
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
    return this.http
      .get<object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(formatErrors)
      );
  }

  findById(id: number | string): Observable<CashflowDetail | undefined> {
    return this.select(state =>
      state.cashflows.find(cashflow => cashflow.id === id)
    );
  }

  modify(cashflow: ModifyCashflowCommand): Observable<void> {
    return this.http.post(`${this.apiUrl}/${apiRoute}/modify`, cashflow).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() =>
        this.messages.sendMessage({
          type: MessageType.cashflow,
          action: MessageAction.post,
          content: { id: cashflow.id },
        })
      )
    );
  }

  cancel(id: string): Observable<void> {
    return this.http.post(`${this.apiUrl}/${apiRoute}/cancel`, { id: id }).pipe(
      switchMap(() => of(void 0)),
      catchError(formatErrors),
      tap(() =>
        this.messages.sendMessage({
          type: MessageType.cashflow,
          action: MessageAction.post,
          content: { id: id },
        })
      )
    );
  }

  private load() {
    this.getAll().subscribe(pagingInfo =>
      this.setState({ cashflows: pagingInfo.items })
    );
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
        catchError(formatErrors)
      );
  }
}
