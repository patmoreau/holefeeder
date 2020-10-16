import { ISeries } from '../interfaces/series.interface';

export class Series implements ISeries {
    from: Date;
    to: Date;
    count: number;
    amount: number;

    constructor(from: Date | string, to: Date | string, count: number, amount: number) {
        Object.assign(this, {
            from: new Date(from),
            to: new Date(to),
            count: count,
            amount: amount
        });
    }
}
