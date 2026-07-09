import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared/models';

export class CloseAccountCommand {
  constructor(public id: string) {}
}

@Injectable({ providedIn: 'root' })
export class CloseAccountAdapter implements Adapter<CloseAccountCommand> {
  adapt(item: { id: string }): CloseAccountCommand {
    return new CloseAccountCommand(item.id);
  }
}
