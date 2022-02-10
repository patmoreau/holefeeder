import { dateFromUtc } from "@app/shared/date-parser.helper";
import { Adapter } from '@app/shared/interfaces/adapter.interface';
import { Injectable } from '@angular/core';
import { ICategoryInfo } from "@app/shared/interfaces/category-info.interface";
import { IAccountInfo } from "@app/shared/interfaces/account-info.interface";

export class Upcoming {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public tags: string[],
    public category: ICategoryInfo,
    public account: IAccountInfo
  ) { }
}

@Injectable({ providedIn: "root" })
export class UpcomingAdapter implements Adapter<Upcoming> {
  adapt(item: any): Upcoming {
    return new Upcoming(item.id, dateFromUtc(item.date), item.amount, item.description, item.tags, item.category, item.account);
  }
}
