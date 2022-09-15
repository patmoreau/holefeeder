import { Injectable } from '@angular/core';
import { Adapter } from '@app/shared';
import { Series } from './series.model';
import { SeriesAdapter } from '@app/core/models/series-adapter.service';

export class Statistics<T> {
  constructor(
    public item: T,
    public yearly: Series[],
    public monthly: Series[],
    public period: Series[]
  ) {}
}

@Injectable({ providedIn: 'root' })
export class StatisticsAdapter<T> implements Adapter<Statistics<T>> {
  constructor(private adapter: SeriesAdapter) {}

  adapt(item: any): Statistics<T> {
    return new Statistics<T>(
      item.item,
      item.yearly.map(this.adapter.adapt),
      item.monthly.map(this.adapter.adapt),
      item.period.map(this.adapter.adapt)
    );
  }
}
