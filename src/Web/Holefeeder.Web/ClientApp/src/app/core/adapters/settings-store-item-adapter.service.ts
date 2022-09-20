import { Injectable } from '@angular/core';
import { nameofFactory } from '@app/shared/helpers';
import { Adapter, DateIntervalType, Settings } from '@app/shared/models';
import { startOfToday } from 'date-fns';

const nameof = nameofFactory<Settings>();

@Injectable({ providedIn: 'root' })
export class SettingsStoreItemAdapter implements Adapter<Settings> {
  adapt(item: any): Settings {
    if (item?.data === undefined) {
      return new Settings(startOfToday(), DateIntervalType.monthly, 1);
    }
    const object = JSON.parse(item.data, (key: string, value: string) => {
      if (key === nameof('effectiveDate')) {
        return new Date(
          +value.substring(0, 4),
          +value.substring(5, 7) - 1,
          +value.substring(8, 10),
          0,
          0,
          0,
          0
        );
      }
      return value;
    });
    return new Settings(
      object.effectiveDate,
      object.intervalType,
      object.frequency
    );
  }
}
