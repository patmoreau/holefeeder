export class ModifyTransactionCommand {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public accountId: string,
    public categoryId: string,
    public tags: string[]
  ) {}
}
