import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { MessageService } from '@app/core/services/message.service';
import { StateService } from '@app/core/services/state.service';
import { filterNullish } from '@app/shared/helpers';
import { MessageType } from '@app/shared/models';
import { catchError, filter, map, Observable, take } from 'rxjs';
import { Account, AccountAdapter, PagingInfo } from '../models';
import { formatErrors, mapToPagingInfo } from '../utils/api.utils';

const apiRoute: string = 'api/v2/accounts';

interface AccountState {
  accounts: Account[];
  selected: Account | null;
}

const initialState: AccountState = {
  accounts: [],
  selected: null,
};

@Injectable({ providedIn: 'root' })
export class AccountsService extends StateService<AccountState> {
  inactiveAccounts$: Observable<Account[]> = this.select(state =>
    state.accounts.filter(x => x.inactive)
  );
  activeAccounts$: Observable<Account[]> = this.select(state =>
    state.accounts.filter(x => !x.inactive)
  );
  count$: Observable<number> = this.select(state => state.accounts.length);
  selectedAccount$: Observable<Account | null> = this.select(
    state => state.selected
  );

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private messages: MessageService,
    private adapter: AccountAdapter
  ) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.account ||
            message.type === MessageType.transaction
        )
      )
      .subscribe(_ => {
        this.load();
      });

    this.load();
  }

  findById(id: string): Observable<Account | undefined> {
    return this.select(state =>
      state.accounts.find(account => account.id === id)
    );
  }

  selectAccount(account: Account) {
    this.setState({ selected: account });
  }

  findOneById(id: string): Observable<Account> {
    return this.select(state => state.accounts.find(x => x.id === id)).pipe(
      take(1),
      filterNullish()
    );
  }

  findOneByIndex(index: number): Account | null {
    if (index < 0 || index > this.state.accounts.length) {
      return null;
    }
    return this.state.accounts[index];
  }

  private load() {
    this.getAll().subscribe(pagingInfo =>
      this.setState({ accounts: pagingInfo.items })
    );
  }

  private getAll(): Observable<PagingInfo<Account>> {
    let params = new HttpParams()
      .append('sort', '-favorite')
      .append('sort', 'name');

    return this.http
      .get<Object[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(formatErrors)
      );
  }
}
