import { Injectable } from '@angular/core';
import { StateService } from '@app/core/services/state.service';
import { combineLatest, map, Observable, Subject, switchMap } from 'rxjs';
import { Upcoming } from '../models/upcoming.model';
import { UpcomingApiService } from './api/upcoming-api.service';
import { SettingsService } from './settings.service';

interface UpcomingState {
  upcoming: Upcoming[];
}

const initialState: UpcomingState = {
  upcoming: undefined,
};

@Injectable({
  providedIn: 'root',
})
export class UpcomingService extends StateService<UpcomingState> {
  private refresh$ = new Subject<void>();

  upcoming$: Observable<Upcoming[]> = this.select((state) => state.upcoming);

  constructor(private apiService: UpcomingApiService, private settingsService: SettingsService) {
    super(initialState);

    combineLatest([
      this.settingsService.period$,
      this.refresh$
    ]).pipe(
      map(([period, _]) => period),
      switchMap(period => this.apiService.findUpcoming(period))
    ).subscribe(items => this.setState({ upcoming: items }));

    this.refresh();
  }

  getUpcoming(accountId: string): Observable<Upcoming[]> {
    return this.select(state => state.upcoming.filter(u => u.account.id === accountId));
  }

  refresh() {
    this.refresh$.next();
  }
}
