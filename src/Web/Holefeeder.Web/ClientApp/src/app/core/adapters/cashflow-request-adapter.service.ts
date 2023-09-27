import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, CashflowRequest, DateIntervalType } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class CashflowRequestAdapter implements Adapter<CashflowRequest> {
  adapt(item: {
    effectiveDate: Date;
    intervalType: DateIntervalType;
    frequency: number;
    recurrence: number;
  }): CashflowRequest {
    return new CashflowRequest(
      dateToUtc(item.effectiveDate),
      item.intervalType,
      item.frequency
    );
  }
}
