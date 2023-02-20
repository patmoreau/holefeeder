import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import {
  DateInterval,
  DateIntervalType,
  MessageAction,
  MessageType,
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
import { mergeMap, Observable, of } from 'rxjs';
import { filter, map, switchMap } from 'rxjs/operators';
import { SettingsStoreItemAdapter, StoreItemAdapter } from '../adapters';
import { StateService } from './state.service';

import { MessageService } from './message.service';

const apiRoute = 'api/v2/store-items';

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
export class SettingsService extends StateService<SettingsState> {
  settings$: Observable<Settings> = this.select(state => state.settings);
  period$: Observable<DateInterval> = this.select(state => state.period);

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private storeItemAdapter: StoreItemAdapter,
    private adapter: SettingsStoreItemAdapter,
    private messages: MessageService
  ) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.general &&
            message.action === MessageAction.userLogOn
        ),
        switchMap(_ => this.getStoreItem(storeItemCode))
      )
      .subscribe(item => {
        const settings = this.adapter.adapt(item);
        const period = this.calculatePeriod(startOfToday(), settings);
        this.resetState(period, settings, item);
      });

    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.general &&
            message.action === MessageAction.userLogOff
        ),
        switchMap(_ => of(initialState))
      )
      .subscribe(state => {
        this.resetState(state.period, state.settings, state.storeItem);
      });
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

  private calculateNextDate(asOfDate: Date, settings: Settings): Date {
    if (settings.intervalType === DateIntervalType.oneTime) {
      return startOfDay(settings.effectiveDate);
    }

    let start = startOfDay(settings.effectiveDate);

    let count = 0;
    while (compareAsc(start, asOfDate) === -1) {
      start = this.nextReccurence(count, settings);
      count++;
    }
    return startOfDay(start);
  }

  private calculatePeriod(asOfDate: Date, settings: Settings): DateInterval {
    let start = startOfDay(asOfDate);
    let end = this.calculateNextDate(start, settings);
    switch (settings.intervalType) {
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
      settings.intervalType === DateIntervalType.oneTime ||
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
    switch (settings.intervalType) {
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

  private getStoreItem(code: string): Observable<StoreItem> {
    return this.http
      .get(`${this.apiUrl}/${apiRoute}?filter=code:eq:${code}`)
      .pipe(
        mergeMap((data: any) => data),
        map((item: any) => this.storeItemAdapter.adapt(item))
      );
  }

  private saveStoreItem(item: StoreItem): Observable<StoreItem> {
    const command = item.id ? 'modify-store-item' : 'create-store-item';
    return this.http.post(`${this.apiUrl}/${apiRoute}/${command}`, item).pipe(
      map((data: any) => {
        if (data?.id === undefined) {
          return item;
        }
        return this.storeItemAdapter.adapt({
          id: data.id,
          code: item.code,
          data: item.data,
        });
      })
    );
  }
}
