import { Injectable } from '@angular/core';
import { StateService } from '@app/core/services/state.service';
import { MessageType } from '@app/shared/enums/message-type.enum';
import { combineLatest, filter, map, Observable, Subject, switchMap, tap } from 'rxjs';
import { Upcoming } from '../models/upcoming.model';
import { UpcomingApiService } from './api/upcoming-api.service';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';

interface UpcomingState {
  upcoming: Upcoming[];
}

const initialState: UpcomingState = {
  upcoming: [],
};

@Injectable({ providedIn: 'root' })
export class UpcomingService extends StateService<UpcomingState> {
  private refresh$ = new Subject<void>();

  upcoming$: Observable<Upcoming[]> = this.select((state) => state.upcoming);

  constructor(private apiService: UpcomingApiService, private settingsService: SettingsService, private messages: MessageService) {
    super(initialState);

    this.messages.listen
    .pipe(
      filter(message => message.type === MessageType.transaction || message.type === MessageType.cashflow)
    ).subscribe(_ => this.refresh());

    combineLatest([
      this.settingsService.period$,
      this.refresh$,
    ]).pipe(
      map(([period, _]) => period),
      switchMap(period => this.apiService.findUpcoming(period)),
    ).subscribe(items => this.setState({ upcoming: items }));

    this.refresh();
  }

  getUpcoming(accountId: string): Observable<Upcoming[]> {
    return this.select(state => state.upcoming.filter(u => u.account.id === accountId));
  }

  getById(id: string): Observable<Upcoming> {
    return this.select(state => state.upcoming.find(u => u.id === id));
  }

  refresh() {
    this.refresh$.next();
  }
}
