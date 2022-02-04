import { Injectable } from "@angular/core";
import { MessageService } from "@app/core/services/message.service";
import { MessageType } from "@app/core/models/message-type.enum";
import { StateService } from "@app/core/services/state.service";
import { combineLatest, filter, map, Observable, switchMap } from "rxjs";
import { Account } from "../models/account.model";
import { ModifyAccountAdapter } from "../models/modify-account-command.model";
import { OpenAccountAdapter } from "../models/open-account-command.model";
import { AccountsApiService } from "./api/accounts-api.service";

interface AccountState {
  accounts: Account[];
  selected: Account;
  count: number;
  inactive: boolean;
}

const initialState: AccountState = {
  accounts: undefined,
  selected: undefined,
  count: 0,
  inactive: false
};

@Injectable({
  providedIn: 'root',
})
export class AccountsService extends StateService<AccountState> {

  private inactive$: Observable<boolean> = this.select((state) => state.inactive);

  accounts$: Observable<Account[]> = this.select((state) => state.accounts);

  count$: Observable<number> = this.select((state) => state.count);

  selectedAccount$: Observable<Account> = this.select((state) => state.selected);

  constructor(private apiService: AccountsApiService, private messages: MessageService, private openAdapter: OpenAccountAdapter, private modifyAdapter: ModifyAccountAdapter) {
    super(initialState);

    combineLatest([
      this.messages.listen.pipe(
        filter(message => message.type === MessageType.account || message.type === MessageType.transaction),
      ),
      this.inactive$
    ]).subscribe(_ => {
      this.refresh()
    });

    this.refresh();
  }

  private refresh() {
    this.apiService.find(null, null, ['-favorite', 'name'], [this.state.inactive ? 'inactive:eq:true' : 'inactive:eq:false'])
      .subscribe(pagingInfo => {
        this.setState({
          accounts: pagingInfo.items,
          count: pagingInfo.totalCount
        });
      })
  }

  findById(id: string): Observable<Account> {
    return this.select((state) => state.accounts?.find(account => account.id === id));
  }

  showInactive() {
    this.setState({ inactive: true });
  }

  hideInactive() {
    this.setState({ inactive: false });
  }

  selectAccount(account: Account) {
    this.setState({ selected: account });
  }

  save(account: Account): Observable<string> {
    if (account.id === undefined) {
      return this.apiService.open(this.openAdapter.adapt(account));
    } else {
      return this.apiService.modify(this.modifyAdapter.adapt(account)).pipe(map(_ => account.id));
    }
  }
}
