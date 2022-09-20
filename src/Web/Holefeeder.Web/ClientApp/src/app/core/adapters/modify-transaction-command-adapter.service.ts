import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, ModifyTransactionCommand } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class ModifyTransactionCommandAdapter
  implements Adapter<ModifyTransactionCommand>
{
  adapt(item: any): ModifyTransactionCommand {
    return new ModifyTransactionCommand(
      item.id,
      dateToUtc(item.date),
      item.amount,
      item.description,
      item.account,
      item.category,
      item.tags
    );
  }
}
