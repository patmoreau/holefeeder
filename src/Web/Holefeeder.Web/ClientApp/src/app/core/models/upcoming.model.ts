import { Injectable } from '@angular/core';
import { Adapter, IAccountInfo, ICategoryInfo } from '@app/shared';
import { dateFromUtc } from '@app/shared/helpers';

export class Upcoming {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public tags: string[],
    public category: ICategoryInfo,
    public account: IAccountInfo
  ) {}
}

@Injectable({ providedIn: 'root' })
export class UpcomingAdapter implements Adapter<Upcoming> {
  adapt(item: any): Upcoming {
    return new Upcoming(
      item.id,
      dateFromUtc(item.date),
      item.amount,
      item.description,
      item.tags,
      item.category,
      item.account
    );
  }
}
