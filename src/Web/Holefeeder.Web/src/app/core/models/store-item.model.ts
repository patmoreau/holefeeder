import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared/interfaces/adapter.interface";

export class StoreItem {
  constructor(
    public id: string,
    public code: string,
    public data: string) { }
}

@Injectable({
  providedIn: "root",
})
export class StoreItemAdapter implements Adapter<StoreItem> {
  adapt(item: any): StoreItem {
    return new StoreItem(item.id, item.code, item.data);
  }
}
