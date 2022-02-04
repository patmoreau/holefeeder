import { Injectable } from '@angular/core';
import { StateService } from '@app/core/services/state.service';
import { filter, mergeMap, Observable, switchMap } from 'rxjs';
import { AccountInfo } from '../models/account-info.model';
import { MessageAction } from '../models/message-action.enum';
import { MessageType } from '../models/message-type.enum';
import { Message } from '../models/message.model';
import { AccountsInfoApiService } from './api/accounts-info-api.service';
import { MessageService } from './message.service';

interface AccountsInfoState {
  accounts: AccountInfo[];
}

const initialState: AccountsInfoState = {
  accounts: undefined,
};

@Injectable({
  providedIn: 'root',
})
export class AccountsInfoService extends StateService<AccountsInfoState> {

  accounts$: Observable<AccountInfo[]> = this.select((state) => state.accounts);

  constructor(private apiService: AccountsInfoApiService, private messages: MessageService) {
    super(initialState);

    this.messages.listen
      .pipe(
        filter(message => message.type === MessageType.account || message.type === MessageType.general)
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

  findOneByIndex(index: number): AccountInfo {
    if (this.state.accounts === undefined) {
      return undefined;
    }
    return this.state.accounts[index];
  }
}
