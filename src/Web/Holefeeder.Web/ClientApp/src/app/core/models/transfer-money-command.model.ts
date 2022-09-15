export class TransferMoneyCommand {
  constructor(
    public date: Date,
    public amount: number,
    public description: string,
    public fromAccountId: string,
    public toAccountId: string
  ) {}
}

