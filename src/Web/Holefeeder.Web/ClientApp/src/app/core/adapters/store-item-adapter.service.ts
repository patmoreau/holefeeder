import { Injectable } from '@angular/core';
import { Adapter, StoreItem } from '@app/shared/models';

export type storeItemType = {
  id: string | null;
  code: string;
  data: string;
};
@Injectable({ providedIn: 'root' })
export class StoreItemAdapter implements Adapter<StoreItem> {
  adapt(item: storeItemType): StoreItem {
    return new StoreItem(item.id, item.code, item.data);
  }
}
