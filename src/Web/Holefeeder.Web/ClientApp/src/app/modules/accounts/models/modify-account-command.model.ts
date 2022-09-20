import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared/models';

export class ModifyAccountCommand {
  constructor(
    public id: string,
    public name: string,
    public openBalance: number,
    public description: string
  ) {}
}

@Injectable({ providedIn: 'root' })
export class ModifyAccountAdapter implements Adapter<ModifyAccountCommand> {
  adapt(item: any): ModifyAccountCommand {
    return new ModifyAccountCommand(
      item.id,
      item.name,
      item.openBalance,
      item.description
    );
  }
}
