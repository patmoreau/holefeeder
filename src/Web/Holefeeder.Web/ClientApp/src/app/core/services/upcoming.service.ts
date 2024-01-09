import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { filterNullish } from '@app/shared/helpers';
import { DateInterval, MessageType, Upcoming } from '@app/shared/models';
import { format } from 'date-fns';
import { filter, map, Observable, take } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { UpcomingAdapter, upcomingType } from '../adapters';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';
import { StateService } from './state.service';

const apiRoute = 'cashflows/get-upcoming';

interface UpcomingState {
  upcoming: Upcoming[];
}

const initialState: UpcomingState = {
  upcoming: [],
};

@Injectable({ providedIn: 'root' })
export class UpcomingService extends StateService<UpcomingState> {
  upcoming$: Observable<Upcoming[]> = this.select(state => state.upcoming);

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private settingsService: SettingsService,
    private messages: MessageService,
    private adapter: UpcomingAdapter
  ) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.transaction ||
            message.type === MessageType.cashflow
        )
      )
      .subscribe(() => this.load());

    this.load();
  }

  getUpcoming(accountId: string): Observable<Upcoming[]> {
    return this.select(state =>
      state.upcoming.filter(u => u.account.id === accountId)
    );
  }

  getById(id: string, date: Date): Observable<Upcoming> {
    return this.select(state =>
      state.upcoming.find(
        u => u.id === id && u.date.toISOString() === date.toISOString()
      )
    ).pipe(take(1), filterNullish());
  }

  private load() {
    this.settingsService.period$
      .pipe(switchMap(period => this.getAll(period)))
      .subscribe(items => this.setState({ upcoming: items }));
  }

  private getAll(period: DateInterval): Observable<Upcoming[]> {
    return this.http
      .get<upcomingType[]>(`${this.apiUrl}/${apiRoute}`, {
        params: new HttpParams()
          .set('from', format(period.start, 'yyyy-MM-dd'))
          .set('to', format(period.end, 'yyyy-MM-dd')),
      })
      .pipe(map(data => data.map(this.adapter.adapt)));
  }
}
