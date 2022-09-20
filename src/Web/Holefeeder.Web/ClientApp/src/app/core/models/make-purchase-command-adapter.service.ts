import { Injectable } from '@angular/core';
import { CashflowRequestAdapter } from '@app/core/models/cashflow-request-adapter.service';
import { MakePurchaseCommand } from '@app/core/models/make-purchase-command.model';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class MakePurchaseCommandAdapter
  implements Adapter<MakePurchaseCommand>
{
  constructor(private adapter: CashflowRequestAdapter) {}

  adapt(item: any): MakePurchaseCommand {
    return new MakePurchaseCommand(
      dateToUtc(item.date),
      item.amount,
      item.description,
      item.account,
      item.category,
      item.tags,
      item.cashflow !== null ? this.adapter.adapt(item.cashflow) : null
    );
  }
}
