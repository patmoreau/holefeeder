import { DateIntervalType } from '@app/shared/models';

export class Settings {
  constructor(
    public effectiveDate: Date,
    public intervalType: DateIntervalType,
    public frequency: number
  ) {}
}
