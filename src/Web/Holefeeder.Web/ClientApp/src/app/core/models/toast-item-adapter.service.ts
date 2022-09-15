import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import {
  DangerToastItem,
  InfoToastItem,
  ToastItem,
  ToastType,
  WarningToastItem
} from "@app/core/models/toast-item.model";

@Injectable({ providedIn: "root" })
export class ToastItemAdapter implements Adapter<ToastItem> {
  adapt(item: any): ToastItem {
    if (item.type === ToastType.danger) {
      return new DangerToastItem(item.message, item.delay);
    } else if (item.type === ToastType.warning) {
      return new WarningToastItem(item.message, item.delay);
    } else {
      return new InfoToastItem(item.message, item.delay);
    }
  }
}
