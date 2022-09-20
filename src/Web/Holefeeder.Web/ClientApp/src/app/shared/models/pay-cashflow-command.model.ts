export class PayCashflowCommand {
  constructor(
    public date: Date,
    public amount: number,
    public cashflowId: string,
    public cashflowDate: Date
  ) {}
}

