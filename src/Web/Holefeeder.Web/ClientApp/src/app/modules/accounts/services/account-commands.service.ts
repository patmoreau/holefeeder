import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, map, switchMap} from 'rxjs/operators';
import {Observable, of} from 'rxjs';
import {OpenAccountCommand} from '../models/open-account-command.model';
import {formatErrors} from '@app/core/utils/api.utils';
import {ModifyAccountCommand} from '../models/modify-account-command.model';

const apiRoute: string = 'budgeting/api/v2/accounts';

@Injectable()
export class AccountCommandsService {
  constructor(private http: HttpClient, @Inject('BASE_API_URL') private apiUrl: string) {
  }

  open(account: OpenAccountCommand): Observable<string> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/open-account`, account)
      .pipe(
        map((data: any) => data.id),
        catchError(formatErrors)
      );
  }

  modify(account: ModifyAccountCommand): Observable<void> {
    return this.http
      .post(`${this.apiUrl}/${apiRoute}/modify-account`, account)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(formatErrors)
      );
  }
}
