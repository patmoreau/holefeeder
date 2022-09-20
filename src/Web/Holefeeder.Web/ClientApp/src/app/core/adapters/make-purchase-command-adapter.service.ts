import { Injectable } from '@angular/core';
import { CashflowRequestAdapter } from '@app/core/adapters';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, MakePurchaseCommand } from '@app/shared/models';

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
