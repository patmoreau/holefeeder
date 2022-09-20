import { Injectable } from '@angular/core';
import { Adapter, ModifyCashflowCommand } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class ModifyCashflowCommandAdapter
  implements Adapter<ModifyCashflowCommand>
{
  adapt(item: any): ModifyCashflowCommand {
    return new ModifyCashflowCommand(
      item.id,
      item.amount,
      item.description,
      item.tags
    );
  }
}
