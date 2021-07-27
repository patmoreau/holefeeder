export class ModifyAccountCommand {
  id: string;
  name: string;
  openBalance: number;
  description: string;

  constructor(obj: {id: string, name: string, openBalance: number, description: string}) {
    this.id = obj.id;
    this.name = obj.name;
    this.openBalance = obj.openBalance;
    this.description = obj.description;
  }
}
