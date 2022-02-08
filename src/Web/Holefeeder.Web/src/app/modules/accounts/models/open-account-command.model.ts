import { Injectable } from "@angular/core";
import { dateToUtc } from "@app/shared/date-parser.helper";
import { AccountType } from "@app/shared/enums/account-type.enum";
import { Adapter } from "@app/shared/interfaces/adapter.interface";

export class OpenAccountCommand {
  constructor(
    public type: AccountType,
    public name: string,
    public openDate: Date,
    public openBalance: number,
    public description: string
  ) { }
}

@Injectable()
export class OpenAccountAdapter implements Adapter<OpenAccountCommand> {
  adapt(item: any): OpenAccountCommand {
    return new OpenAccountCommand(item.type, item.name, dateToUtc(item.openDate), item.openBalance, item.description);
  }
}
