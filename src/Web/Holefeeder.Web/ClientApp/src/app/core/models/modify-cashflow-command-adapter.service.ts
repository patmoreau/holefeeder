import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { ModifyCashflowCommand } from "@app/core/models/modify-cashflow-command.model";

@Injectable({ providedIn: "root" })
export class ModifyCashflowCommandAdapter
  implements Adapter<ModifyCashflowCommand> {
  adapt(item: any): ModifyCashflowCommand {
    return new ModifyCashflowCommand(
      item.id,
      item.amount,
      item.description,
      item.tags
    );
  }
}
