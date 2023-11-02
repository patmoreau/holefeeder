import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { formatErrors, mapToPagingInfo } from '@app/core/utils/api.utils';
import { PagingInfo, TransactionDetail } from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TransactionDetailAdapter } from '../../adapters';

const apiRoute = 'api/v2/transactions';

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: TransactionDetailAdapter
  ) {}

  fetch(
    accountId: string,
    offset: number,
    limit: number,
    sort: string[]
  ): Observable<PagingInfo<TransactionDetail>> {
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
