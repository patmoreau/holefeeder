import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { IStatistics } from '../interfaces/statistics.interface';
import { ICategoryInfo } from '@app/shared/interfaces/category-info.interface';
import { ISettings } from '@app/shared/interfaces/settings.interface';
import { HttpParams } from '@angular/common/http';
import { Series } from '../models/series.model';
import { Statistics } from '../models/statistics.model';
import { map } from 'rxjs/operators';

@Injectable()
export class StatisticsService {

  private basePath = 'api/v1/categories';

  constructor(private api: ApiService) { }

  find(settings: ISettings): Promise<IStatistics<ICategoryInfo>[]> {
    let params = new HttpParams();
    params = params.set('effectiveDate', `${settings.effectiveDate.toISOString()}`);
    params = params.set('intervalType', `${settings.intervalType}`);
    params = params.set('frequency', `${settings.intervalType}`);
    return this.api
      .get(`${this.basePath}\\statistics`, params)
      .pipe(
        map(data =>
          data.map(s =>
            new Statistics<ICategoryInfo>(
              s.item,
              s.yearly.map(x => new Series(x.from, x.to, x.count, x.amount)),
              s.monthly.map(x => new Series(x.from, x.to, x.count, x.amount)),
              s.period.map(x => new Series(x.from, x.to, x.count, x.amount))
            )
          )
        )
      )
      .toPromise();
  }

  findByCategoryId(id: string, settings: ISettings): Promise<IStatistics<ICategoryInfo>[]> {
    let params = new HttpParams();
    params = params.set('effectiveDate', `${settings.effectiveDate.toISOString()}`);
    params = params.set('intervalType', `${settings.intervalType}`);
    params = params.set('frequency', `${settings.intervalType}`);
    return this.api
      .get(`${this.basePath}\\${id}\\statistics`, params)
      .pipe(
        map(data =>
          data.map(s =>
            new Statistics<ICategoryInfo>(
              s.item,
              s.yearly.map(x => new Series(x.from, x.to, x.count, x.amount)),
              s.monthly.map(x => new Series(x.from, x.to, x.count, x.amount)),
              s.period.map(x => new Series(x.from, x.to, x.count, x.amount))
            )
          )
        )
      )
      .toPromise();
  }
}
