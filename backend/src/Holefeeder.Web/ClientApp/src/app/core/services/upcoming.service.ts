import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnDestroy, inject } from '@angular/core';
import { filterNullish } from '@app/shared/helpers';
import { DateInterval, MessageType, MessageAction, Upcoming } from '@app/shared/models';
import { format } from 'date-fns';
import { filter, map, Observable, take, debounceTime, shareReplay, takeUntil, Subject, catchError, throwError } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { UpcomingAdapter, upcomingType } from '../adapters';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';
import { StateService } from './state.service';

const apiRoute = 'cashflows/get-upcoming';
const DEBOUNCE_TIME = 300; // ms

interface UpcomingState {
  upcoming: Upcoming[];
  lastUpdate: number;
}

const initialState: UpcomingState = {
  upcoming: [],
  lastUpdate: 0
};

@Injectable({ providedIn: 'root' })
export class UpcomingService extends StateService<UpcomingState> implements OnDestroy {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);
  private settingsService = inject(SettingsService);
  private messages = inject(MessageService);
  private adapter = inject(UpcomingAdapter);

  upcoming$: Observable<Upcoming[]> = this.select(state => state.upcoming);
  private readonly destroy$ = new Subject<void>();

  constructor() {
    super(initialState);
    this.initializeSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    super.destroy();
  }

  private initializeSubscriptions(): void {
    // Listen for messages that indicate data refresh is needed
    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.transaction ||
            message.type === MessageType.cashflow
        ),
        debounceTime(DEBOUNCE_TIME),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.load());

    // Initial load
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
    // Prevent multiple loads within 1 second
    const now = Date.now();
    if (now - this.state.lastUpdate < 1000) {
      return;
    }

    this.settingsService.period$
      .pipe(
        switchMap(period => this.getAll(period)),
        take(1),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: items => this.setState({
          upcoming: items,
          lastUpdate: now
        }),
        error: error => {
          console.error('Failed to load upcoming data:', error);
          // Keep the previous state on error
        }
      });
  }

  private getAll(period: DateInterval): Observable<Upcoming[]> {
    return this.http
      .get<upcomingType[]>(`${this.apiUrl}/${apiRoute}`, {
        params: new HttpParams()
          .set('from', format(period.start, 'yyyy-MM-dd'))
          .set('to', format(period.end, 'yyyy-MM-dd')),
      })
      .pipe(
        map(data => data.map(this.adapter.adapt)),
        catchError(error => {
          console.error('HTTP error in getAll upcoming:', error);
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load upcoming transactions. Please try again later.'
          });
          return throwError(() => error);
        }),
        shareReplay(1)
      );
  }
}
