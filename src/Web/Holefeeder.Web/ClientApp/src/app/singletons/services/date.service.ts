import { Injectable } from '@angular/core';
import { ISettings } from '@app/shared/interfaces/settings.interface';
import { IDateInterval } from '@app/shared/interfaces/date-interval.interface';
import {
  addYears,
  addMonths,
  addWeeks,
  addDays,
  startOfDay,
  startOfToday,
  compareAsc
} from 'date-fns';
import { SettingsService } from '@app/singletons/services/settings.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { DateIntervalType } from '@app/shared/enums/date-interval-type.enum';

@Injectable({ providedIn: 'root' })
export class DateService {
  private settings: ISettings;
  private period$: BehaviorSubject<IDateInterval>;

  constructor(private objectDataService: SettingsService) {
    this.period$ = new BehaviorSubject({
      start: startOfToday(),
      end: addMonths(startOfToday(), 1)
    });
    this.objectDataService.settings.subscribe(settings => {
      this.settings = settings;
      this.setPeriod(startOfToday());
    });
  }

  getNextDate(asOfDate: Date): Date {
    if (this.settings.intervalType === DateIntervalType.oneTime) {
      return startOfDay(this.settings.effectiveDate);
    }

    let start = startOfDay(this.settings.effectiveDate);

    let count = 0;
    while (compareAsc(start, asOfDate) === -1) {
      start = this.nextReccurence(count);
      count++;
    }
    return startOfDay(start);
  }

  getPreviousDate(asOfDate: Date): Date {
    if (
      this.settings.intervalType === DateIntervalType.oneTime ||
      compareAsc(asOfDate, this.settings.effectiveDate) !== 1
    ) {
      return startOfDay(this.settings.effectiveDate);
    }

    let start = startOfDay(this.settings.effectiveDate);

    let count = 0;
    while (compareAsc(start, asOfDate) === -1) {
      count++;
      start = this.nextReccurence(count);
    }
    return this.nextReccurence(count - 1);
  }

  get period(): Observable<IDateInterval> {
    return this.period$.asObservable();
  }

  setPeriod(asOfDate: Date | IDateInterval) {
    const period =
      asOfDate instanceof Date
        ? this.getPeriod(asOfDate)
        : (asOfDate as IDateInterval);
    this.period$.next(period);
  }

  getPeriod(asOfDate: Date): IDateInterval {
    let start = startOfDay(asOfDate);
    let end = this.getNextDate(start);
    switch (this.settings.intervalType) {
      case DateIntervalType.weekly:
        if (compareAsc(start, end) === 0) {
          end = addWeeks(end, this.settings.frequency);
        } else {
          start = addWeeks(end, -this.settings.frequency);
        }
        break;
      case DateIntervalType.monthly:
        if (compareAsc(start, end) === 0) {
          end = addMonths(end, this.settings.frequency);
        } else {
          start = addMonths(end, -this.settings.frequency);
        }
        break;
      case DateIntervalType.yearly:
        if (compareAsc(start, end) === 0) {
          end = addYears(end, this.settings.frequency);
        } else {
          start = addYears(end, -this.settings.frequency);
        }
        break;
    }
    return { start: start, end: addDays(end, -1) };
  }

  private nextReccurence(amount: number): Date {
    switch (this.settings.intervalType) {
      case DateIntervalType.weekly:
        return addWeeks(
          this.settings.effectiveDate,
          this.settings.frequency * amount
        );
      case DateIntervalType.monthly:
        return addMonths(
          this.settings.effectiveDate,
          this.settings.frequency * amount
        );
      case DateIntervalType.yearly:
        return addYears(
          this.settings.effectiveDate,
          this.settings.frequency * amount
        );
      case DateIntervalType.oneTime:
      default:
        return startOfDay(this.settings.effectiveDate);
    }
  }
}
