import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { trace } from '@app/core/logger';
import { MessageService } from '@app/core/services';
import { formatErrors } from '@app/core/utils/api.utils';
import { CloseAccountCommand } from '@app/modules/accounts/models/close-account-command.model';
import { MessageAction, MessageType } from '@app/shared/models';
import { Observable, of, tap } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ModifyAccountCommand } from '../models/modify-account-command.model';
import { OpenAccountCommand } from '../models/open-account-command.model';

const apiRoute: string = 'api/v2/accounts';

@Injectable({ providedIn: 'root' })
export class AccountCommandsService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private apiUrl: string,
    private messages: MessageService
  ) {}

  open(account: OpenAccountCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/open-account`, account)
      .pipe(
        map((data: any) => data.id),
        tap((id: string) =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: id,
          })
        ),
        catchError(formatErrors)
      );
  }

  @trace()
  modify(account: ModifyAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/modify-account`, account)
      .pipe(
        tap(_ =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: account.id,
          })
        ),
        switchMap(_ => of(void 0)),
        catchError(formatErrors)
      );
  }

  close(account: CloseAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/close-account`, account)
      .pipe(
        tap(_ =>
          this.messages.sendMessage({
            type: MessageType.account,
            action: MessageAction.post,
            content: account.id,
          })
        ),
        switchMap(_ => of(void 0)),
        catchError(formatErrors)
      );
  }
}
