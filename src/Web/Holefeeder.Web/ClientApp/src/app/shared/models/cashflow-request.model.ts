import { DateIntervalType } from '@app/shared/models';

export class CashflowRequest {
  constructor(
    public effectiveDate: Date,
    public intervalType: DateIntervalType,
    public frequency: number,
    public recurrence: number = 0
  ) {}
}

