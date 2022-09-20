import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { AccountType, Adapter } from '@app/shared/models';

export class OpenAccountCommand {
  constructor(
    public type: AccountType,
    public name: string,
    public openDate: Date,
    public openBalance: number,
    public description: string
  ) {}
}

@Injectable({ providedIn: 'root' })
export class OpenAccountAdapter implements Adapter<OpenAccountCommand> {
  adapt(item: any): OpenAccountCommand {
    return new OpenAccountCommand(
      item.type,
      item.name,
      dateToUtc(item.openDate),
      item.openBalance,
      item.description
    );
  }
}
