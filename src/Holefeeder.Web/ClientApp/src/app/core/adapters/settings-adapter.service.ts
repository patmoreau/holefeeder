import { Injectable } from '@angular/core';
import { Adapter, DateIntervalType, Settings } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class SettingsAdapter implements Adapter<Settings> {
  adapt(item: {
    effectiveDate: Date;
    intervalType: DateIntervalType;
    frequency: number;
  }): Settings {
    return new Settings(item.effectiveDate, item.intervalType, item.frequency);
  }
}
