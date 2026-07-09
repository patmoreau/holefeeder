import { Injectable, inject } from '@angular/core';
import { Statistics } from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';
import { Summary } from '@app/shared/models/summary.model';
import { format } from 'date-fns';

export const apiRoute = 'categories/statistics';
export const apiSummaryRoute = 'summary/statistics';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  private http = inject(HttpClient);
  apiUrl = inject(BASE_API_URL);


  find(): Observable<Statistics[]> {
    return this.getStatistics();
  }

  private getStatistics(): Observable<Statistics[]> {
    return this.http
      .get<Statistics[]>(`${this.apiUrl}/${apiRoute}`)
      .pipe(
        catchError(error => {
          console.error('HTTP error in getStatistics:', error);
          return formatErrors(error);
        })
      );
  }

  fetchSummary(from: Date, to: Date): Observable<Summary> {
    return this.http
      .get<Summary>(`${this.apiUrl}/${apiSummaryRoute}`, {
        params: new HttpParams()
          .set('from', format(from, 'yyyy-MM-dd'))
          .set('to', format(to, 'yyyy-MM-dd')),
      })
      .pipe(
        catchError(error => {
          console.error('HTTP error in fetchSummary:', error);
          return formatErrors(error);
        })
      );
  }
}
