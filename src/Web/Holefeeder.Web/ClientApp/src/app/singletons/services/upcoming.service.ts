import {Injectable} from '@angular/core';
import {HttpParams} from '@angular/common/http';
import {format} from 'date-fns';
import {ApiService} from '@app/shared/services/api.service';
import {map, switchMap, tap} from 'rxjs/operators';
import {Observable, BehaviorSubject, Subject, merge} from 'rxjs';
import {DateService} from './date.service';
import {IDateInterval} from "@app/shared/interfaces/date-interval.interface";
import {IUpcoming, upcomingFromServer} from "@app/shared/interfaces/v2/upcoming.interface";

@Injectable({providedIn: 'root'})
export class UpcomingService {
  private basePath = 'api/v2/cashflows/get-upcoming';

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
        map(data => Object.assign(
          [],
          data.map(upcomingFromServer)
          )
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
