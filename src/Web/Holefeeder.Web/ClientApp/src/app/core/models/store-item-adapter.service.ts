import { Injectable } from '@angular/core';
import { StoreItem } from '@app/core/models/store-item.model';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class StoreItemAdapter implements Adapter<StoreItem> {
  adapt(item: any): StoreItem {
    return new StoreItem(item.id, item.code, item.data);
  }
}
