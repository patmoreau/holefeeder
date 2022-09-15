import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { StoreItem } from "@app/core/models/store-item.model";

@Injectable({ providedIn: "root" })
export class StoreItemAdapter implements Adapter<StoreItem> {
  adapt(item: any): StoreItem {
    return new StoreItem(item.id, item.code, item.data);
  }
}
