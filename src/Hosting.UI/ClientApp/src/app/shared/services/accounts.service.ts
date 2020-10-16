import { Injectable } from '@angular/core';

import { IAccount, accountToServer, accountFromServer } from '@app/shared/interfaces/account.interface';
import { ApiService } from '@app/shared/services/api.service';
import { HttpParams } from '@angular/common/http';
import { IAccountDetail, accountDetailFromServer } from '../interfaces/account-detail.interface';
import { map } from 'rxjs/operators';

@Injectable()
export class AccountsService {
  private basePath = 'api/v1/accounts';

  constructor(private api: ApiService) {}

  find(
    offset: number,
    limit: number,
    sort: string[],
    filter: string[]
  ): Promise<IAccountDetail[]> {
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
    return this.api
      .get(this.basePath, params)
      .pipe(
        map(data =>
          Object.assign(
            [],
            data.map(accountDetailFromServer)
          )
        )
      )
      .toPromise();
  }

  findOneById(id: number | string): Promise<IAccount> {
    return this.api
      .get(`${this.basePath}/${id}`)
      .pipe(
        map(accountFromServer)
      )
      .toPromise();
  }

  findOneByIdWithDetails(id: number | string): Promise<IAccountDetail> {
    return this.api
      .get(`${this.basePath}/${id}/details`)
      .pipe(
        map(accountDetailFromServer)
      )
      .toPromise();
  }

  create(account: IAccount): Promise<IAccount> {
    return this.api
      .post(`${this.basePath}`, accountToServer(account))
      .pipe(
        map(accountFromServer)
      )
      .toPromise();
  }

  update(id: number | string, account: IAccount): Promise<void> {
    return this.api.put(`${this.basePath}/${id}`, accountToServer(account)).toPromise();
  }
}
