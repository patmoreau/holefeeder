import { AccountType } from '@app/shared/models';
import { dateToUtc } from '@app/shared/helpers';

export class OpenAccountCommand {
  constructor(
    public type: AccountType,
    public name: string,
    public openDate: Date,
    public openBalance: number,
    public description: string
  ) {}

  static fromObject(obj: {
    type: AccountType;
    name: string;
    openDate: Date;
    openBalance: number;
    description: string;
  }): OpenAccountCommand {
    return new OpenAccountCommand(
      obj.type,
      obj.name,
      dateToUtc(obj.openDate),
      obj.openBalance,
      obj.description
    );
  }
}
