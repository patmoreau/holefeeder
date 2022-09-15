import { IAccountInfo, ICashflowInfo, ICategoryInfo } from '@app/shared';

export class TransactionDetail {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public category: ICategoryInfo,
    public account: IAccountInfo,
    public cashflow: ICashflowInfo,
    public tags: string[]
  ) {}
}

