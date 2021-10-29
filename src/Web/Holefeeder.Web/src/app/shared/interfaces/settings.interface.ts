import { DateIntervalType } from '../enums/date-interval-type.enum';
import { dateToUtc, dateFromUtc } from '../date-parser.helper';
import { Settings } from '../models/settings.model';

export interface ISettings {
    effectiveDate: Date;
    intervalType: DateIntervalType;
    frequency: number;
}

export function settingsToServer(item: ISettings): ISettings {
    return Object.assign({} as ISettings, item, {
        effectiveDate: dateToUtc(item.effectiveDate)
    });
}

export function settingsFromServer(item: ISettings): ISettings {
    return Object.assign(new Settings(), item, {
        effectiveDate: dateFromUtc(item.effectiveDate)
    });
}
