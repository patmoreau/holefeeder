import {Inject, Injectable} from '@angular/core';
import {catchError, map} from 'rxjs/operators';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {BaseApiService} from './base-api.service';
import {PagingInfo} from '@app/core/models/paging-info.model';
import {CashflowDetail, CashflowDetailAdapter} from '@app/core/models/cashflow-detail.model';

const apiRoute: string = 'budgeting/api/v2/cashflows';

@Injectable({providedIn: 'root'})
export class CashflowsApiService extends BaseApiService {

  constructor(private http: HttpClient, @Inject('BASE_API_URL') private apiUrl: string, private adapter: CashflowDetailAdapter) {
    super();
  }

  find(offset: number, limit: number, sort: string[], filter: string[]): Observable<PagingInfo<CashflowDetail>> {
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
      .get<Object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params
      }).pipe(
        map(resp => this.mapToPagingInfo(resp, this.adapter)),
        catchError(this.formatErrors)
      );
  }

  findOneById(id: number | string): Observable<CashflowDetail> {
    return this.http
      .get(`${this.apiUrl}/${apiRoute}/${id}`)
      .pipe(
        map(this.adapter.adapt),
        catchError(this.formatErrors)
      );
  }
}
