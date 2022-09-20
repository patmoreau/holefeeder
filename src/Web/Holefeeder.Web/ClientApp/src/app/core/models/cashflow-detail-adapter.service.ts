import { Injectable } from '@angular/core';
import { CashflowDetail } from '@app/core/models/cashflow-detail.model';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class CashflowDetailAdapter implements Adapter<CashflowDetail> {
  adapt(item: any): CashflowDetail {
    return new CashflowDetail(
      item.id,
      dateFromUtc(item.effectiveDate),
      item.amount,
      item.intervalType,
      item.frequency,
      item.recurrence,
      item.description,
      item.category,
      item.account,
      item.inactive,
      item.tags
    );
  }
}
