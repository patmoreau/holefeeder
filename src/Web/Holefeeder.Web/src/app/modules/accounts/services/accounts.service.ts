import { Injectable } from "@angular/core";
import { MessageService } from "@app/core/services/message.service";
import { StateService } from "@app/core/services/state.service";
import { combineLatest, filter, map, Observable, tap } from "rxjs";
import { Account } from "../models/account.model";
import { ModifyAccountAdapter, ModifyAccountCommand } from "../models/modify-account-command.model";
import { OpenAccountAdapter, OpenAccountCommand } from "../models/open-account-command.model";
import { AccountsApiService } from "./api/accounts-api.service";
import { MessageAction } from "@app/shared/enums/message-action.enum";
import { MessageType } from "@app/shared/enums/message-type.enum";

interface AccountState {
  accounts: Account[];
  selected: Account | null;
  count: number;
  inactive: boolean;
}

const initialState: AccountState = {
  accounts: [],
  selected: null,
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

  selectedAccount$: Observable<Account | null> = this.select((state) => state.selected);

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

  findById(id: string): Observable<Account | null> {
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

  open(account: OpenAccountCommand): Observable<string> {
    return this.apiService.open(account)
      .pipe(tap(id => this.messages.sendMessage(MessageType.account, MessageAction.post, { id: id })));
  }

  modify(account: ModifyAccountCommand): Observable<string> {
    return this.apiService.modify(account)
      .pipe(
        map(_ => account.id),
        tap(id => this.messages.sendMessage(MessageType.account, MessageAction.post, { id: id }))
      );
  }
}
