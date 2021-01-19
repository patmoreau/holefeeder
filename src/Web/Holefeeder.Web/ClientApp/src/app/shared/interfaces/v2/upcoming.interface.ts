import {ICategoryInfo} from './category-info.interface';
import {IAccountInfo} from './account-info.interface';
import {dateFromUtc, dateToUtc} from "@app/shared/date-parser.helper";

export interface IUpcoming {
  id: string;
  date: Date;
  amount: number;
  description: string;
  category: ICategoryInfo;
  account: IAccountInfo;
}

export function upcomingToServer(item: IUpcoming): IUpcoming {
  return Object.assign({} as IUpcoming, item, {
    date: dateToUtc(item.date)
  });
}

export function upcomingFromServer(item: IUpcoming): IUpcoming {
  return Object.assign({} as IUpcoming, item, {
    date: dateFromUtc(item.date)
  });
}
