import { Injectable } from '@angular/core';
import { Adapter, ModifyCashflowCommand } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class ModifyCashflowCommandAdapter
  implements Adapter<ModifyCashflowCommand>
{
  adapt(item: {
    id: string;
    amount: number;
    effectiveDate: Date;
    description: string;
    tags: string[];
  }): ModifyCashflowCommand {
    return new ModifyCashflowCommand(
      item.id,
      item.amount,
      item.effectiveDate,
      item.description,
      item.tags
    );
  }
}
