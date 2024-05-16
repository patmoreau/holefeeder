import { Injectable } from '@angular/core';
import { AccountInfo, Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class AccountInfoAdapter implements Adapter<AccountInfo> {
  adapt(item: { id: string; name: string }): AccountInfo {
    return new AccountInfo(item.id, item.name);
  }
}
