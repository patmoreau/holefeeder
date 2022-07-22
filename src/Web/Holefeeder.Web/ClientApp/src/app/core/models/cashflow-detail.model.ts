import {Injectable} from "@angular/core";
import {dateFromUtc} from "@app/shared/date-parser.helper";
import {DateIntervalType} from "@app/shared/enums/date-interval-type.enum";
import {IAccountInfo} from "@app/shared/interfaces/account-info.interface";
import {Adapter} from "@app/shared/interfaces/adapter.interface";
import {ICategoryInfo} from "@app/shared/interfaces/category-info.interface";

export class CashflowDetail {
  constructor(
    public id: string,
    public effectiveDate: Date,
    public amount: number,
    public intervalType: DateIntervalType,
    public frequency: number,
    public recurrence: number,
    public description: string,
    public category: ICategoryInfo,
    public account: IAccountInfo,
    public inactive: boolean,
    public tags: string[]
  ) {
  }
}

@Injectable({providedIn: 'root'})
export class CashflowDetailAdapter implements Adapter<CashflowDetail> {
  adapt(item: any): CashflowDetail {
    return new CashflowDetail(item.id, dateFromUtc(item.effectiveDate), item.amount, item.intervalType,
      item.frequency, item.recurrence, item.description, item.category, item.account, item.inactive, item.tags);
  }
}
