import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, map, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { ConfigService } from '@app/core/config/config.service';
import { PagingInfo } from '@app/core/models/paging-info.model';
import { Account, AccountAdapter } from '../../models/account.model';
import { BaseApiService } from '@app/core/services/api/base-api.service';
import { OpenAccountCommand } from '../../models/open-account-command.model';
import { ModifyAccountCommand } from '../../models/modify-account-command.model';

const apiRoute: string = 'budgeting/api/v2/accounts';

@Injectable()
export class AccountsApiService extends BaseApiService {
  constructor(private http: HttpClient, private configService: ConfigService, private adapter: AccountAdapter) {
    super();
  }

  find(offset: number | null, limit: number | null, sort: string[], filter: string[]): Observable<PagingInfo<Account>> {
    let params = new HttpParams();
    if (offset) {
      params = params.set('offset', `${offset}`);
    }
    if (limit) {
      params = params.set('limit', `${limit}`);
    }
    if (sort) {
      sort.forEach(element => {
        params = params.append('sort', `${element}`);
      });
    }
    if (filter) {
      filter.forEach(element => {
        params = params.append('filter', `${element}`);
      });
    }
    return this.http
      .get<Object[]>(`${this.configService.config.apiUrl}/${apiRoute}`, {
        observe: 'response',
        params: params
      }).pipe(
        map(resp => this.mapToPagingInfo(resp, this.adapter)),
        catchError(this.formatErrors)
      );
  }

  findOneByIdWithDetails(id: string): Observable<Account> {
    return this.http
      .get(`${this.configService.config.apiUrl}/${apiRoute}/${id}`)
      .pipe(
        map(this.adapter.adapt),
        catchError(this.formatErrors)
      );
  }

  open(account: OpenAccountCommand): Observable<string> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/open-account`, account)
      .pipe(
        map((data: any) => data.id),
        catchError(this.formatErrors)
      );
  }

  modify(account: ModifyAccountCommand): Observable<void> {
    return this.http
      .post(`${this.configService.config.apiUrl}/${apiRoute}/modify-account`, account)
      .pipe(
        switchMap(_ => of(void 0)),
        catchError(this.formatErrors)
      );
  }
}
