import { Injectable } from '@angular/core';
import { Series } from '@app/core/models/series.model';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class SeriesAdapter implements Adapter<Series> {
  adapt(item: any): Series {
    return new Series(
      new Date(item.from),
      new Date(item.to),
      item.count,
      item.amount
    );
  }
}
