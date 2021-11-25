import { dateToUtc } from "../date-parser.helper";

export class OpenAccountCommand {
  type: string;
  name: string;
  openDate: Date;
  openBalance: number;
  description: string;

  constructor(obj: {type: string, name: string, openDate: Date, openBalance: number, description: string}) {
    this.type = obj.type;
    this.name = obj.name;
    this.openDate = dateToUtc(obj.openDate);
    this.openBalance = obj.openBalance;
    this.description = obj.description;
  }

}
