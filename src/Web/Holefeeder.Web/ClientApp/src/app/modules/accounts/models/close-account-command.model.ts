import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export class CloseAccountCommand {
  constructor(
    public id: string,
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class CloseAccountAdapter implements Adapter<CloseAccountCommand> {
  adapt(item: any): CloseAccountCommand {
    return new CloseAccountCommand(item.id);
  }
}
