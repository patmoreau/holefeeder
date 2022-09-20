import { Injectable } from '@angular/core';
import { DateInterval } from '@app/core/models/date-interval.model';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class DateIntervalAdapter implements Adapter<DateInterval> {
  adapt(item: any): DateInterval {
    return new DateInterval(item.start, item.end);
  }
}
