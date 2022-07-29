import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, map, switchMap} from 'rxjs/operators';
import {Observable, of, tap} from 'rxjs';
import {OpenAccountCommand} from '../models/open-account-command.model';
import {formatErrors} from '@app/core/utils/api.utils';
import {ModifyAccountCommand} from '../models/modify-account-command.model';
import {CloseAccountCommand} from "@app/modules/accounts/models/close-account-command.model";
import {MessageService} from "@app/core";
import {LoggerService} from "@app/core/logger/logger.service";
import {MessageAction, MessageType} from "@app/shared";

const apiRoute: string = 'api/v2/accounts';

@Injectable()
export class AccountCommandsService {
  constructor(private http: HttpClient,
              @Inject('BASE_API_URL') private apiUrl: string,
              private messages: MessageService,
              private logger: LoggerService
  ) {
  }

  open(account: OpenAccountCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/open-account`, account)
      .pipe(
        map((data: any) => data.id),
        tap((id: string) => this.messages.sendMessage(MessageType.account, MessageAction.post, id)),
        catchError(formatErrors)
      );
  }

  modify(account: ModifyAccountCommand): Observable<void> {
    this.logger.logInfo(account);
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/modify-account`, account)
      .pipe(
        tap(_ => this.messages.sendMessage(MessageType.account, MessageAction.post, account.id)),
        switchMap(_ => of(void 0)),
        catchError(formatErrors)
      );
  }

  close(account: CloseAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/close-account`, account)
      .pipe(
        tap(_ => this.messages.sendMessage(MessageType.account, MessageAction.post, account.id)),
        switchMap(_ => of(void 0)),
        catchError(formatErrors)
      );
  }
}
