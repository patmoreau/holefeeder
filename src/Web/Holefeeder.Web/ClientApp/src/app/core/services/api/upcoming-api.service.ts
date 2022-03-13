import {Inject, Injectable} from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { format } from 'date-fns';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Upcoming, UpcomingAdapter } from "@app/core/models/upcoming.model";
import { DateInterval } from '../../models/date-interval.model';

const apiRoute: string = 'budgeting/api/v2/cashflows/get-upcoming';

@Injectable({ providedIn: 'root' })
export class UpcomingApiService {
  constructor(private http: HttpClient, @Inject('BASE_API_URL') private apiUrl: string, private adapter: UpcomingAdapter) {
  }

  findUpcoming(period: DateInterval): Observable<Upcoming[]> {
    return this.http.get(`${this.apiUrl}/${apiRoute}`, {
      params: new HttpParams()
        .set('from', format(period.start, 'yyyy-MM-dd'))
        .set('to', format(period.end, 'yyyy-MM-dd'))
    })
      .pipe(
        map((data: any) => data.map(this.adapter.adapt))
      );
  }
}
