import {Injectable} from '@angular/core';
import {Adapter} from '@app/shared';

export class AccountInfo {
  constructor(
    public id: string,
    public name: string,
  ) {
  }
}

@Injectable({providedIn: "root"})
export class AccountInfoAdapter implements Adapter<AccountInfo> {
  adapt(item: any): AccountInfo {
    return new AccountInfo(item.id, item.name);
  }
}
