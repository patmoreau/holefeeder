import { CashflowRequest } from '@app/core/models/cashflow-request.model';

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

