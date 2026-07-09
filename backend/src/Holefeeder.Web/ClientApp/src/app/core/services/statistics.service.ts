import { Injectable, inject } from '@angular/core';
import { Statistics } from '@app/shared/models';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { formatErrors } from '@app/core/utils/api.utils';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';

const apiRoute = 'categories/statistics';

@Injectable({ providedIn: 'root' })
export class StatisticsService {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);


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
}
