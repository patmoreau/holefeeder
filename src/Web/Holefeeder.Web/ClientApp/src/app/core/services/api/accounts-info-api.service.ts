import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ConfigService } from '@app/core/config/config.service';
import { BaseApiService } from '@app/core/services/api/base-api.service';
import { AccountInfo, AccountInfoAdapter } from '@app/core/models/account-info.model';

const apiRoute: string = 'budgeting/api/v2/accounts';

@Injectable({ providedIn: 'root' })
export class AccountsInfoApiService extends BaseApiService {
  constructor(private http: HttpClient, private configService: ConfigService, private adapter: AccountInfoAdapter) {
    super();
  }

  find(): Observable<AccountInfo[]> {
    let params = new HttpParams()
      .append('filter', `inactive:eq:false`)
      .append('sort', '-favorite')
      .append('sort', 'name');
    return this.http
      .get<Object[]>(`${this.configService.config.apiUrl}/${apiRoute}`, {
        params: params
      }).pipe(
        map((data: any[]) => data.map(this.adapter.adapt)),
        catchError(this.formatErrors)
      );
  }
}
