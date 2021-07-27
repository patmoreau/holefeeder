import { ISettings } from '../interfaces/settings.interface';
import { DateIntervalType } from '../enums/date-interval-type.enum';

export class Settings implements ISettings {
    effectiveDate: Date;
    intervalType: DateIntervalType;
    frequency: number;
}
