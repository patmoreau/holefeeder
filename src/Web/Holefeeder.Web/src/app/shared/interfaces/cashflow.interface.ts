import { DateIntervalType } from '../enums/date-interval-type.enum';
import { Cashflow } from '../models/cashflow.model';
import { dateToUtc, dateFromUtc } from '../date-parser.helper';

export interface ICashflow {
    id: string;
    intervalType: DateIntervalType;
    effectiveDate: Date;
    amount: number;
    frequency: number;
    recurrence: number;
    description: string;
    account: string;
    category: string;
    tags: Array<string>;
}

export function cashflowToServer(item: ICashflow): ICashflow {
    return Object.assign({} as ICashflow, item, {
        effectiveDate: dateToUtc(item.effectiveDate)
    });
}

export function cashflowFromServer(item: ICashflow): ICashflow {
    return Object.assign(new Cashflow(), item, {
        effectiveDate: dateFromUtc(item.effectiveDate)
    });
}
