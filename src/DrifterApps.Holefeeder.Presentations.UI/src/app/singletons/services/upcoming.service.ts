import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { format } from 'date-fns';
import { IUpcoming, upcomingFromServer } from '../../shared/interfaces/upcoming.interface';
import { ApiService } from '@app/shared/services/api.service';
import { map, switchMap, tap } from 'rxjs/operators';
import { Observable, BehaviorSubject, Subject, merge } from 'rxjs';
import { IDateInterval } from '../../shared/interfaces/date-interval.interface';
import { DateService } from './date.service';

@Injectable({ providedIn: 'root' })
export class UpcomingService {
  private basePath = 'api/v1/cashflows/upcoming';

  private period: IDateInterval;

  private cashflows$ = new BehaviorSubject<IUpcoming[]>([]);
  private refresh$ = new Subject<IDateInterval>();

  constructor(private dateService: DateService, private api: ApiService) {
    merge(this.dateService.period, this.refresh$.asObservable())
      .pipe(
        tap(period => this.period = period),
        switchMap(period => this.findUpcoming(period))
      ).subscribe(cashflows => this.cashflows$.next(cashflows));
  }

  findUpcoming(period: IDateInterval): Observable<IUpcoming[]> {
    return this.api
      .get(
        this.basePath,
        new HttpParams()
          .set('from', format(period.start, 'yyyy-MM-dd'))
          .set('to', format(period.end, 'yyyy-MM-dd'))
      )
      .pipe(
        map(data =>{
          console.log('upcomingService', data);
          return Object.assign(
            [],
            data.map(upcomingFromServer)
          );
        }
        )
      );
  }

  refreshUpcoming() {
    this.refresh$.next(this.period);
  }

  get cashflows(): Observable<IUpcoming[]> {
    return this.cashflows$.asObservable();
  }
}
