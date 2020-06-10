import { ISeries } from '../interfaces/series.interface';
import { IStatistics } from '../interfaces/statistics.interface';

export class Statistics<T> implements IStatistics<T> {
    item: T;
    yearly: ISeries[];
    monthly: ISeries[];
    period: ISeries[];

    constructor(item: T, yearly: ISeries[], monthly: ISeries[], period: ISeries[]) {
        Object.assign(this, {
            item: item,
            yearly: yearly,
            monthly: monthly,
            period: period,
        });
    }
}
