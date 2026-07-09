import { StatisticsYearly } from '@app/shared/models/statistics-yearly.model';

export class Statistics {
  constructor(
    public categoryId: string,
    public category: string,
    public color: string,
    public monthlyAverage: number,
    public years: StatisticsYearly[]
  ) {}
}
