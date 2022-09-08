import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';

export class ModifyCashflowCommand {
  constructor(
    public id: string,
    public amount: number,
    public description: string,
    public tags: string[]
  ) {}
}

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
