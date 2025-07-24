import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable, OnDestroy } from '@angular/core';
import { MessageService } from '@app/core/services';
import {
  Account,
  accountTypeMultiplier,
  categoryTypeMultiplier,
  MessageType,
  MessageAction,
  PagingInfo,
  Upcoming,
} from '@app/shared/models';
import { catchError, filter, map, Observable, shareReplay, timer, debounceTime, takeUntil, Subject } from 'rxjs';
import { AccountAdapter, accountType } from '@app/core/adapters';
import { formatErrors, mapToPagingInfo } from '../utils/api.utils';
import { StateService } from './state.service';

const apiRoute = 'accounts';
const CACHE_TTL = 60000; // 1 minute cache
const DEBOUNCE_TIME = 300; // ms

interface AccountState {
  accounts: Account[];
  selected: Account | null;
  lastUpdate: number;
}

const initialState: AccountState = {
  accounts: [],
  selected: null,
  lastUpdate: 0
};

@Injectable({ providedIn: 'root' })
export class AccountsService extends StateService<AccountState> implements OnDestroy {
  inactiveAccounts$: Observable<Account[]> = this.select(state =>
    state.accounts.filter(x => x.inactive)
  );
  activeAccounts$: Observable<Account[]> = this.select(state =>
    state.accounts.filter(x => !x.inactive)
  );
  selectedAccount$: Observable<Account | null> = this.select(
    state => state.selected
  );

  private readonly destroy$ = new Subject<void>();

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private messages: MessageService,
    private adapter: AccountAdapter
  ) {
    super(initialState);
    this.initializeSubscriptions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    super.destroy();
  }

  private initializeSubscriptions(): void {
    // Listen for messages that indicate data refresh is needed
    this.messages.listen
      .pipe(
        filter(
          message =>
            message.type === MessageType.account ||
            message.type === MessageType.transaction
        ),
        debounceTime(DEBOUNCE_TIME),
        takeUntil(this.destroy$)
      )
      .subscribe(() => {
        this.load();
      });

    // Set up periodic refresh
    timer(CACHE_TTL, CACHE_TTL)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.load());

    // Initial load
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

  findOneByIndex(index: number): Account | null {
    if (index < 0 || index > this.state.accounts.length) {
      return null;
    }
    return this.state.accounts[index];
  }

  private load() {
    const now = Date.now();
    // Skip if data is fresh enough
    if (now - this.state.lastUpdate < DEBOUNCE_TIME) {
      return;
    }

    this.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: pagingInfo => this.setState({
          accounts: pagingInfo.items,
          lastUpdate: now
        }),
        error: (error: HttpErrorResponse) => {
          console.error('Failed to load accounts:', error);
          this.messages.sendMessage({
            type: MessageType.error,
            action: MessageAction.error,
            content: 'Failed to load accounts. Please try again later.'
          });
        }
      });
  }

  private getAll(): Observable<PagingInfo<Account>> {
    const params = new HttpParams()
      .append('sort', '-favorite')
      .append('sort', 'name');

    return this.http
      .get<accountType[]>(`${this.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params,
      })
      .pipe(
        map(resp => mapToPagingInfo(resp, this.adapter)),
        catchError(error => {
          console.error('HTTP error in getAll accounts:', error);
          return formatErrors(error);
        }),
        shareReplay(1)
      );
  }
  public getUpcomingBalance(account: Account, cashflows: Upcoming[]): number {
    return (
      account.balance +
      (cashflows
        ? cashflows
          .filter(cashflow => cashflow.account.id === account.id)
          .map(cashflow => {
            return (
              cashflow.amount *
              categoryTypeMultiplier(cashflow.category.type) *
              accountTypeMultiplier(account.type)
            );
          })
          .reduce((sum, current) => sum + current, 0)
        : 0)
    );
  }
}
