import { Injectable } from '@angular/core';
import { Adapter, Settings } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class SettingsAdapter implements Adapter<Settings> {
  adapt(item: any): Settings {
    return new Settings(item.effectiveDate, item.intervalType, item.frequency);
  }
}
