import { Injectable } from '@angular/core';
import { AccountInfo, Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class AccountInfoAdapter implements Adapter<AccountInfo> {
  adapt(item: any): AccountInfo {
    return new AccountInfo(item.id, item.name);
  }
}
