import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { Settings } from "@app/core/models/settings.model";

@Injectable({ providedIn: "root" })
export class SettingsAdapter implements Adapter<Settings> {
  adapt(item: any): Settings {
    return new Settings(item.effectiveDate, item.intervalType, item.frequency);
  }
}
