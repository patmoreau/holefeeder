import { AccountType } from '@app/shared/models';

export class Account {
  constructor(
    public id: string,
    public name: string,
    public type: AccountType,
    public openBalance: number,
    public openDate: Date,
    public transactionCount: number,
    public balance: number,
    public updated: Date,
    public description: string,
    public favorite: boolean,
    public inactive: boolean
  ) {}
}
