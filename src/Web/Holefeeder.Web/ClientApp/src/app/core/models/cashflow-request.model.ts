import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared/interfaces/adapter.interface";
import {DateIntervalType} from "@app/shared/enums/date-interval-type.enum";
import {dateToUtc} from "@app/shared/date-parser.helper";

export class CashflowRequest {
  constructor(
    public effectiveDate: Date,
    public intervalType: DateIntervalType,
    public frequency: number,
    public recurrence: number = 0
  ) {
  }
}

@Injectable({providedIn: "root"})
export class CashflowRequestAdapter implements Adapter<CashflowRequest> {
  constructor() {
  }

  adapt(item: any): CashflowRequest {
    return new CashflowRequest(dateToUtc(item.effectiveDate), item.intervalType, item.frequency);
  }
}
