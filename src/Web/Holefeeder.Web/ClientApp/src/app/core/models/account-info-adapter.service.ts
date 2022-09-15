import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { AccountInfo } from "@app/core/models/account-info.model";

@Injectable({ providedIn: "root" })
export class AccountInfoAdapter implements Adapter<AccountInfo> {
  adapt(item: any): AccountInfo {
    return new AccountInfo(item.id, item.name);
  }
}
