import {DateInterval, MessageService, Upcoming, UpcomingAdapter} from "@app/core";
import {Inject, Injectable} from "@angular/core";
import {switchMap} from "rxjs/operators";
import {StateService} from "@app/core/services/state.service";
import {filter, map, Observable, take} from "rxjs";
import {HttpClient, HttpParams} from "@angular/common/http";
import {filterNullish, MessageType} from "@app/shared";
import {SettingsService} from "@app/core/services/settings.service";
import {format} from "date-fns";

const apiRoute: string = 'api/v2/cashflows/get-upcoming';

interface UpcomingState {
  upcoming: Upcoming[];
}

const initialState: UpcomingState = {
  upcoming: [],
};

@Injectable({providedIn: 'root'})
export class UpcomingService extends StateService<UpcomingState> {
  upcoming$: Observable<Upcoming[]> = this.select((state) => state.upcoming);

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
        filter(message => message.type === MessageType.transaction || message.type === MessageType.cashflow)
      ).subscribe(_ => this.load());

    this.load();
  }

  getUpcoming(accountId: string): Observable<Upcoming[]> {
    return this.select(state => state.upcoming.filter(u => u.account.id === accountId));
  }

  getById(id: string, date: Date): Observable<Upcoming> {
    return this.select(state => state.upcoming.find(u => u.id === id && u.date.toISOString() === date.toISOString()))
      .pipe(
        take(1),
        filterNullish(),
      );
  }

  private load() {
    this.settingsService.period$.pipe(
      switchMap(period => this.getAll(period)),
    ).subscribe(items => this.setState({upcoming: items}));
  }

  private getAll(period: DateInterval): Observable<Upcoming[]> {
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
