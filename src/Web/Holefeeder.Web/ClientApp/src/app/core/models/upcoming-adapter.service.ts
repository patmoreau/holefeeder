import { Injectable } from '@angular/core';
import { Upcoming } from '@app/core/models/upcoming.model';
import { dateFromUtc } from '@app/shared/helpers';
import { Adapter } from '@app/shared/models';

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
