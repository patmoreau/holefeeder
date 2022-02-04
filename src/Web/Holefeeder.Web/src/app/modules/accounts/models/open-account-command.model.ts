import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared/interfaces/adapter.interface";
import { dateToUtc } from "../../../shared/date-parser.helper";

export class OpenAccountCommand {
  constructor(
    public type: string,
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
