import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { formatErrors, mapToPagingInfo } from '@app/core/utils/api.utils';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';
import {
  CategoryType,
  PagingInfo,
  TransactionDetail,
} from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TransactionDetailAdapter } from '../../adapters';

const apiRoute = 'transactions';

interface TransactionDetailResponse {
  id: string;
  date: Date;
  amount: number;
  description: string;
  category: {
    id: string;
    name: string;
    type: CategoryType;
    color: string;
  };
  account: {
    id: string;
    name: string;
    mongoId: string;
  };
  cashflow: {
    id: string;
    date: Date;
  };
  tags: string[];
}

@Injectable({ providedIn: 'root' })
export class TransactionsService {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);
  private adapter = inject(TransactionDetailAdapter);


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
      .get<TransactionDetailResponse[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(error => {
          console.error('HTTP error in fetch transactions:', error);
          return formatErrors(error);
        })
      );
  }
}
