import {Injectable} from "@angular/core";
import {Adapter, DateIntervalType, dateToUtc} from "@app/shared";

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
