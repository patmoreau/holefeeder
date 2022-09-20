import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, TransferMoneyCommand } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class TransferMoneyCommandAdapter
  implements Adapter<TransferMoneyCommand>
{
  adapt(item: any): TransferMoneyCommand {
    return new TransferMoneyCommand(
      dateToUtc(item.date),
      item.amount,
      item.description,
      item.fromAccount,
      item.toAccount
    );
  }
}
