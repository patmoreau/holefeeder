export class ModifyCashflowCommand {
  constructor(
    public id: string,
    public amount: number,
    public description: string,
    public tags: string[]
  ) {}
}
