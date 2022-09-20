import { Injectable } from '@angular/core';
import { CashflowRequest } from '@app/core/models/cashflow-request.model';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

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
