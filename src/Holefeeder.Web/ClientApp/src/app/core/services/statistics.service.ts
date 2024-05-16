import { Inject, Injectable } from '@angular/core';
import { Statistics } from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';

const apiRoute = 'categories/statistics';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string
  ) {}

  find(): Observable<Statistics[]> {
    return this.getStatistics();
  }

  private getStatistics(): Observable<Statistics[]> {
    return this.http
      .get<Statistics[]>(`${this.apiUrl}/${apiRoute}`)
      .pipe(catchError(formatErrors));
  }
}
