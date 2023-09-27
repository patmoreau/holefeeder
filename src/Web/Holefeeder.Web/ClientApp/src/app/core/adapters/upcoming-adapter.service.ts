import { Injectable } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import {
  Adapter,
  IAccountInfo,
  ICategoryInfo,
  Upcoming,
} from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class UpcomingAdapter implements Adapter<Upcoming> {
  adapt(item: {
    id: string;
    date: Date;
    amount: number;
    description: string;
    tags: string[];
    category: ICategoryInfo;
    account: IAccountInfo;
  }): Upcoming {
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
