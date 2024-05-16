export class ModifyAccountCommand {
  constructor(
    public id: string,
    public name: string,
    public openBalance: number,
    public description: string
  ) {}

  static fromObject(obj: {
    id: string;
    name: string;
    openBalance: number;
    description: string;
  }): ModifyAccountCommand {
    return new ModifyAccountCommand(
      obj.id || '',
      obj.name || '',
      obj.openBalance || 0,
      obj.description || ''
    );
  }
}
