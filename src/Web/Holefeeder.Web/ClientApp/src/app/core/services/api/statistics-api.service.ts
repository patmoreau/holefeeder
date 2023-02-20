import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import {
  ICategoryInfo,
  Settings,
  Statistics,
  StatisticsAdapter,
} from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { BaseApiService } from './base-api.service';

const apiRoute = 'api/v2/categories';

@Injectable({ providedIn: 'root' })
export class StatisticsApiService extends BaseApiService {
  private basePath = 'api/v1/categories';

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private adapter: StatisticsAdapter<ICategoryInfo>
  ) {
    super();
  }

  find(settings: Settings): Observable<Statistics<ICategoryInfo>[]> {
    const params = new HttpParams()
      .set('effectiveDate', `${settings.effectiveDate.toISOString()}`)
      .set('intervalType', `${settings.intervalType}`)
      .set('frequency', `${settings.intervalType}`);

    return this.http
      .get<Object[]>(`${this.apiUrl}/${apiRoute}/statistics`, {
        params: params,
      })
      .pipe(
        map((data: any[]) => data.map(this.adapter.adapt)),
        catchError(this.formatErrors)
      );
  }

  findByCategoryId(
    id: string,
    settings: Settings
  ): Observable<Statistics<ICategoryInfo>[]> {
    const params = new HttpParams()
      .set('effectiveDate', `${settings.effectiveDate.toISOString()}`)
      .set('intervalType', `${settings.intervalType}`)
      .set('frequency', `${settings.intervalType}`);

    return this.http
      .get<Object[]>(`${this.apiUrl}/${apiRoute}/${id}/statistics`, {
        params: params,
      })
      .pipe(
        map((data: any[]) => data.map(this.adapter.adapt)),
        catchError(this.formatErrors)
      );
  }
}
