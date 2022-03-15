import {Inject, Injectable} from '@angular/core';
import {ICategoryInfo} from '@app/shared/interfaces/category-info.interface';
import {Settings} from '@app/core/models/settings.model';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Statistics, StatisticsAdapter} from '../../models/statistics.model';
import {catchError, map} from 'rxjs/operators';
import {Observable} from 'rxjs';
import {BaseApiService} from './base-api.service';

const apiRoute: string = 'budgeting/api/v2/categories';

@Injectable({providedIn: 'root'})
export class StatisticsApiService extends BaseApiService {

  private basePath = 'api/v1/categories';

  constructor(private http: HttpClient, @Inject('BASE_API_URL') private apiUrl: string, private adapter: StatisticsAdapter<ICategoryInfo>) {
    super();
  }

  find(settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    let params = new HttpParams()
      .set('effectiveDate', `${settings.effectiveDate.toISOString()}`)
      .set('intervalType', `${settings.intervalType}`)
      .set('frequency', `${settings.intervalType}`);

    return this.http
      .get<Object[]>(`${this.apiUrl}/${apiRoute}/statistics`, {
        params: params
      })
      .pipe(
        map((data: any[]) => data.map(this.adapter.adapt)),
        catchError(this.formatErrors)
      );
  }

  findByCategoryId(id: string, settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    let params = new HttpParams()
      .set('effectiveDate', `${settings.effectiveDate.toISOString()}`)
      .set('intervalType', `${settings.intervalType}`)
      .set('frequency', `${settings.intervalType}`);

    return this.http
      .get<Object[]>(`${this.apiUrl}/${apiRoute}/${id}/statistics`, {
        params: params
      })
      .pipe(
        map((data: any[]) => data.map(this.adapter.adapt)),
        catchError(this.formatErrors)
      );
  }
}
