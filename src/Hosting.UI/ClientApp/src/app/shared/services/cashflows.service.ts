import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { ICashflow, cashflowFromServer, cashflowToServer } from '../interfaces/cashflow.interface';
import { map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class CashflowsService {
  private basePath = 'api/v1/cashflows';

  constructor(private api: ApiService) { }

  find(
    offset: number,
    limit: number,
    sort: string[],
    filter: string[]
  ): Promise<ICashflow[]> {
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
            data.map(cashflowFromServer)
          )
        )
      )
      .toPromise();
  }

  findOneById(id: number | string): Promise<ICashflow> {
    return this.api
      .get(`${this.basePath}/${id}`)
      .pipe(
        map(cashflowFromServer)
      )
      .toPromise();
  }

  create(cashflow: ICashflow): Promise<ICashflow> {
    return this.api
      .post(`${this.basePath}`, cashflowToServer(cashflow))
      .pipe(
        map(cashflowFromServer)
      )
      .toPromise();
  }

  update(
    id: number | string,
    cashflow: ICashflow
  ): Promise<void> {
    return this.api
      .put(`${this.basePath}/${id}`, cashflowToServer(cashflow))
      .toPromise();
  }
}
