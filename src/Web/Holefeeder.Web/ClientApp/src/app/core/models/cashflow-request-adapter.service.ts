import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { dateToUtc } from "@app/shared/helpers";
import { CashflowRequest } from "@app/core/models/cashflow-request.model";

@Injectable({ providedIn: "root" })
export class CashflowRequestAdapter implements Adapter<CashflowRequest> {
  constructor() {
  }

  adapt(item: any): CashflowRequest {
    return new CashflowRequest(
      dateToUtc(item.effectiveDate),
      item.intervalType,
      item.frequency
    );
  }
}
