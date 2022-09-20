import { Injectable } from '@angular/core';
import { Settings } from '@app/core/models/settings.model';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class SettingsAdapter implements Adapter<Settings> {
  adapt(item: any): Settings {
    return new Settings(item.effectiveDate, item.intervalType, item.frequency);
  }
}
