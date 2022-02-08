import { Injectable } from '@angular/core';
import { StateService } from '@app/core/services/state.service';
import { MessageType } from '@app/shared/enums/message-type.enum';
import { filter, mergeMap, Observable } from 'rxjs';
import { AccountInfo } from '../models/account-info.model';
import { AccountsInfoApiService } from './api/accounts-info-api.service';
import { MessageService } from './message.service';

interface AccountsInfoState {
  accounts: AccountInfo[];
}

const initialState: AccountsInfoState = {
  accounts: [],
};

@Injectable({ providedIn: 'root' })
export class AccountsInfoService extends StateService<AccountsInfoState> {

  accounts$: Observable<AccountInfo[]> = this.select((state) => state.accounts).pipe(filter(accounts => accounts.length !== 0));

  constructor(private apiService: AccountsInfoApiService, private messages: MessageService) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(message => message.type === MessageType.account || message.type === MessageType.transaction)
      ).subscribe(_ => {
        this.refresh();
      });

    this.refresh();
  }

  private refresh() {
    this.apiService.find()
      .subscribe(items => {
        this.setState({
          accounts: items
        })
      });
  }

  findOneById(id: string): Observable<AccountInfo> {
    return this.select((state) => state.accounts)
      .pipe(
        mergeMap((data: any[]) => data),
        filter(c => c.id === id)
      );
  }

  findOneByIndex(index: number): AccountInfo | null {
    if (index < 0 || index > this.state.accounts.length) {
      return null;
    }
    return this.state.accounts[index];
  }
}
