import { Injectable } from '@angular/core';
import { Adapter, DateInterval } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class DateIntervalAdapter implements Adapter<DateInterval> {
  adapt(item: any): DateInterval {
    return new DateInterval(item.start, item.end);
  }
}
