import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';
import { dateToUtc } from '@app/shared/helpers';

export class ModifyTransactionCommand {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[]
  ) {}
}

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
