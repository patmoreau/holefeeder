import { Inject, Injectable } from '@angular/core';
import { Statistics } from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { Summary } from '@app/shared/models/summary.model';
import { format } from 'date-fns';

export const apiRoute = 'categories/statistics';
export const apiSummaryRoute = 'summary/statistics';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') public apiUrl: string
  ) { }

  find(): Observable<Statistics[]> {
    return this.getStatistics();
  }

  private getStatistics(): Observable<Statistics[]> {
    return this.http
      .get<Statistics[]>(`${this.apiUrl}/${apiRoute}`)
      .pipe(catchError(formatErrors));
  }

  fetchSummary(from: Date, to: Date): Observable<Summary> {
    return this.http
      .get<Summary>(`${this.apiUrl}/${apiSummaryRoute}`, {
        params: new HttpParams()
          .set('from', format(from, 'yyyy-MM-dd'))
          .set('to', format(to, 'yyyy-MM-dd')),
      })
      .pipe(catchError(formatErrors));
  }
}
