import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { dateToUtc } from "@app/shared/helpers";
import { ModifyTransactionCommand } from "@app/core/models/modify-transaction-command.model";

@Injectable({ providedIn: "root" })
export class ModifyTransactionCommandAdapter
  implements Adapter<ModifyTransactionCommand> {
  adapt(item: any): ModifyTransactionCommand {
    return new ModifyTransactionCommand(
      item.id,
      dateToUtc(item.date),
      item.amount,
      item.description,
      item.account,
      item.category,
      item.tags
    );
  }
}
