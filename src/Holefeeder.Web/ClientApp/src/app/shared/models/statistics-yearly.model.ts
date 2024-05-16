import { StatisticsMonthly } from '@app/shared/models/statistics-monthly.model';

export class StatisticsYearly {
  constructor(
    public year: number,
    public total: number,
    public months: StatisticsMonthly[]
  ) {}
}
