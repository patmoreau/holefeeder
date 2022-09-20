import { CashflowRequest } from '@app/shared/models';

export class MakePurchaseCommand {
  constructor(
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[],
    public cashflow: CashflowRequest | null
  ) {}
}
