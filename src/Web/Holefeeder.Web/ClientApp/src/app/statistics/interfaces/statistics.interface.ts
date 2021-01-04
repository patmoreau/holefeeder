import { ISeries } from './series.interface';

export interface IStatistics<T> {
    item: T;
    yearly: ISeries[];
    monthly: ISeries[];
    period: ISeries[];
}
