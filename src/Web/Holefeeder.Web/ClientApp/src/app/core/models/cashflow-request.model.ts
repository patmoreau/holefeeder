import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';
import { dateToUtc } from '@app/shared/helpers';
import { DateIntervalType } from '@app/shared/models';

export class CashflowRequest {
  constructor(
    public effectiveDate: Date,
    public intervalType: DateIntervalType,
    public frequency: number,
    public recurrence: number = 0
  ) {}
}

@Injectable({ providedIn: 'root' })
export class CashflowRequestAdapter implements Adapter<CashflowRequest> {
  constructor() {}

  adapt(item: any): CashflowRequest {
    return new CashflowRequest(
      dateToUtc(item.effectiveDate),
      item.intervalType,
      item.frequency
    );
  }
}
