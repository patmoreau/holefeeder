import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, CashflowRequest } from '@app/shared/models';

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
