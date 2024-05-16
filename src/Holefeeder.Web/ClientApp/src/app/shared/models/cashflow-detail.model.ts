import {
  DateIntervalType,
  IAccountInfo,
  ICategoryInfo,
} from '@app/shared/models';

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
  ) {}
}
