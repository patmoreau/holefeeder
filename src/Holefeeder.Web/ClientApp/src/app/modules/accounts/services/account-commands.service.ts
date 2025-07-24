import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { MessageService } from '@app/core/services';
import { formatErrors } from '@app/core/utils/api.utils';
import { BASE_API_URL } from '@app/core/tokens/injection-tokens';
import { CloseAccountCommand } from '@app/modules/accounts/models/close-account-command.model';
import { MessageAction, MessageType } from '@app/shared/models';
import { Observable, of, tap } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ModifyAccountCommand } from '../models/modify-account-command.model';
import { OpenAccountCommand } from '../models/open-account-command.model';
import { IdResultType } from '@app/shared/types/id-result.type';

const apiRoute = 'accounts';

@Injectable({ providedIn: 'root' })
export class AccountCommandsService {
  private http = inject(HttpClient);
  private apiUrl = inject(BASE_API_URL);
  private messages = inject(MessageService);


  open(account: OpenAccountCommand): Observable<string> {
    return this.http
      .post<IdResultType>(`${this.apiUrl}/${apiRoute}/open-account`, account)
      .pipe(
        map(data => data.id),
        tap((id: string) =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: id,
          })
        ),
        catchError(error => {
          console.error('HTTP error in open account:', error);
          return formatErrors(error);
        })
      );
  }

  modify(account: ModifyAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/modify-account`, account)
      .pipe(
        tap(() =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: account.id,
          })
        ),
        switchMap(() => of(void 0)),
        catchError(error => {
          console.error('HTTP error in modify account:', error);
          return formatErrors(error);
        })
      );
  }

  close(account: CloseAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/close-account`, account)
      .pipe(
        tap(() =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: account.id,
          })
        ),
        switchMap(() => of(void 0)),
        catchError(error => {
          console.error('HTTP error in close account:', error);
          return formatErrors(error);
        })
      );
  }
}
