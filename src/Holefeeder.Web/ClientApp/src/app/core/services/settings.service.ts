import { HttpClient } from '@angular/common/http';
import { inject, Injectable, OnDestroy } from '@angular/core';
import {
  DateInterval,
  DateIntervalType,
  Settings,
  StoreItem,
} from '@app/shared/models';
import {
  addDays,
  addMonths,
  addWeeks,
  addYears,
  compareAsc,
  startOfDay,
  startOfToday,
} from 'date-fns';
import { Observable, of, Subject } from 'rxjs';
import { map, takeUntil, catchError } from 'rxjs/operators';
import {
  SettingsStoreItemAdapter,
  StoreItemAdapter,
  storeItemType,
} from '../adapters';
import { StateService } from './state.service';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';

import { AppState } from '@app/core/store';
import { Store } from '@ngrx/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import { filterTrue } from '@app/shared/helpers';
import { LoggerService } from '@app/core/logger';

const apiRoute = 'store-items';
const MAX_LOOP_ITERATIONS = 1000; // Safety limit to prevent infinite loops

interface SettingsState {
  period: DateInterval;
  settings: Settings;
  storeItem: StoreItem;
}

const storeItemCode = 'settings';

const initialState: SettingsState = {
  period: new DateInterval(startOfToday(), addMonths(startOfToday(), 1)),
  settings: new Settings(startOfToday(), DateIntervalType.monthly, 1),
  storeItem: new StoreItem(null, storeItemCode, ''),
};

@Injectable({ providedIn: 'root' })
export class SettingsService extends StateService<SettingsState> implements OnDestroy {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);
  private storeItemAdapter = inject(StoreItemAdapter);
  private adapter = inject(SettingsStoreItemAdapter);

  settings$: Observable<Settings> = this.select(state => state.settings);
  period$: Observable<DateInterval> = this.select(state => state.period);

  private readonly store = inject(Store<AppState>);
  private readonly logger = inject(LoggerService);
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
    this.store.select(AuthFeature.selectIsAuthenticated)
      .pipe(
        filterTrue(),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.loadSettings());
  }

  getNextDate(asOfDate: Date): Date {
    return this.calculateNextDate(asOfDate, this.state.settings);
  }

  getPeriod(asOfDate: Date): DateInterval {
    return this.calculatePeriod(asOfDate, this.state.settings);
  }

  getPreviousDate(asOfDate: Date): Date {
    return this.calculatePreviousDate(asOfDate, this.state.settings);
  }

  setPeriod(asOfDate: Date | DateInterval) {
    const currState = this.state;

    const period =
      asOfDate instanceof Date
        ? this.calculatePeriod(asOfDate, this.state.settings)
        : asOfDate;

    this.resetState(period, currState.settings, currState.storeItem);
  }

  saveSettings(settings: Settings): Observable<void> {
    const currStoreItem = this.state.storeItem;

    return this.saveStoreItem(
      new StoreItem(
        currStoreItem.id,
        currStoreItem.code,
        JSON.stringify(settings)
      )
    ).pipe(
      map(storeItem => {
        const newSettings = this.adapter.adapt(storeItem);
        const period = this.calculatePeriod(startOfToday(), newSettings);
        this.resetState(period, newSettings, storeItem);
      })
    );
  }

  private loadSettings(): void {
    this.getStoreItem(storeItemCode)
      .pipe(
        takeUntil(this.destroy$),
      )
      .subscribe({
        next: (result) => {
          const settings = this.adapter.adapt(result);
          const period = this.calculatePeriod(startOfToday(), settings);
          this.resetState(period, settings, result);
        },
        error: (error) => {
          this.logger.error('Error loading settings, falling back to initial state', error);
          // Fall back to initial state on any error (timeout, auth failure, etc.)
          this.resetState(initialState.period, initialState.settings, initialState.storeItem);
        },
      });
  }

  private getStoreItem(code: string): Observable<StoreItem> {
    return this.http
      .get<ReadonlyArray<storeItemType>>(
        `${this.apiUrl}/${apiRoute}?filter=code:eq:${code}`
      )
      .pipe(
        map((items: ReadonlyArray<storeItemType>) => {
          if (items.length > 0) {
            return this.storeItemAdapter.adapt(items[0]);
          }
          return new StoreItem(null, code, JSON.stringify(initialState.settings));
        }),
        catchError(error => {
          this.logger.error('Error fetching store item', error);
          // Return default StoreItem on error to keep the stream alive
          return of(new StoreItem(null, code, JSON.stringify(initialState.settings)));
        }),
      );
  }

  private saveStoreItem(item: StoreItem): Observable<StoreItem> {
    const command = item.id ? 'modify-store-item' : 'create-store-item';
    return this.http
      .post<storeItemType>(`${this.apiUrl}/${apiRoute}/${command}`, item)
      .pipe(
        map(data => {
          if (data?.id === undefined) {
            return item;
          }
          return this.storeItemAdapter.adapt(data);
        }),
        catchError(error => {
          this.logger.error('Error saving store item', error);
          // Re-throw the error so the caller can handle it
          throw error;
        })
      );
  }

  private calculateNextDate(asOfDate: Date, settings: Settings): Date {
    const intervalTypeLower = settings.intervalType.toLowerCase();
    if (intervalTypeLower === DateIntervalType.oneTime) {
      return startOfDay(settings.effectiveDate);
    }

    let start = startOfDay(settings.effectiveDate);

    let count = 0;

    while (compareAsc(start, asOfDate) === -1) {
      count++;
      start = this.nextReccurence(count, settings);

      // Safety break to prevent infinite loop during debugging
      if (count > MAX_LOOP_ITERATIONS) {
        this.logger.error('🔧 calculateNextDate: Breaking loop - too many iterations!');
        break;
      }
    }

    return startOfDay(start);
  }

  private calculatePeriod(asOfDate: Date, settings: Settings): DateInterval {
    let start = startOfDay(asOfDate);
    let end = this.calculateNextDate(start, settings);
    const intervalType = settings.intervalType.toLowerCase();

    switch (intervalType) {
      case DateIntervalType.weekly:
        if (compareAsc(start, end) === 0) {
          end = addWeeks(end, settings.frequency);
        } else {
          start = addWeeks(end, -settings.frequency);
        }
        break;
      case DateIntervalType.monthly:
        if (compareAsc(start, end) === 0) {
          end = addMonths(end, settings.frequency);
        } else {
          start = addMonths(end, -settings.frequency);
        }
        break;
      case DateIntervalType.yearly:
        if (compareAsc(start, end) === 0) {
          end = addYears(end, settings.frequency);
        } else {
          start = addYears(end, -settings.frequency);
        }
        break;
    }
    return new DateInterval(start, addDays(end, -1));
  }

  private calculatePreviousDate(asOfDate: Date, settings: Settings): Date {
    if (
      settings.intervalType.toLowerCase() === DateIntervalType.oneTime ||
      compareAsc(asOfDate, settings.effectiveDate) !== 1
    ) {
      return startOfDay(settings.effectiveDate);
    }

    let start = startOfDay(settings.effectiveDate);

    let count = 0;
    while (compareAsc(start, asOfDate) === -1) {
      count++;
      start = this.nextReccurence(count, settings);
    }
    return this.nextReccurence(count - 1, settings);
  }

  private nextReccurence(amount: number, settings: Settings): Date {
    // Convert intervalType to lowercase to match enum values
    const intervalType = settings.intervalType.toLowerCase();

    switch (intervalType) {
      case DateIntervalType.weekly:
        return addWeeks(settings.effectiveDate, settings.frequency * amount);
      case DateIntervalType.monthly:
        return addMonths(settings.effectiveDate, settings.frequency * amount);
      case DateIntervalType.yearly:
        return addYears(settings.effectiveDate, settings.frequency * amount);
      case DateIntervalType.oneTime:
      default:
        return startOfDay(settings.effectiveDate);
    }
  }

  private resetState(
    period: DateInterval,
    settings: Settings,
    storeItem: StoreItem
  ) {
    this.setState({
      period: period,
      settings: settings,
      storeItem: storeItem,
    });
  }
}
