import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';

export class DateInterval {
  constructor(public start: Date, public end: Date) {}
}

@Injectable({ providedIn: 'root' })
export class DateIntervalAdapter implements Adapter<DateInterval> {
  adapt(item: any): DateInterval {
    return new DateInterval(item.start, item.end);
  }
}
