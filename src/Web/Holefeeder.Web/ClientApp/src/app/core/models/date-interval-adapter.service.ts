import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { DateInterval } from "@app/core/models/date-interval.model";

@Injectable({ providedIn: "root" })
export class DateIntervalAdapter implements Adapter<DateInterval> {
  adapt(item: any): DateInterval {
    return new DateInterval(item.start, item.end);
  }
}
