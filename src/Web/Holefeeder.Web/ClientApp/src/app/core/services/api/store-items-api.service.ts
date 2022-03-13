import {Inject, Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, mergeMap, Observable } from 'rxjs';
import { StoreItem, StoreItemAdapter } from '../../models/store-item.model';

const apiRoute: string = 'object-store/api/v2/store-items';

@Injectable({ providedIn: 'root' })
export class StoreItemsService {
  constructor(private http: HttpClient, @Inject('BASE_API_URL') private apiUrl: string, private adapter: StoreItemAdapter) {
  }

  getStoreItem(code: string): Observable<StoreItem> {
    return this.http.get(`${this.apiUrl}/${apiRoute}?filter=code:eq:${code}`)
      .pipe(
        mergeMap((data: any) => data),
        map((item: any) => this.adapter.adapt(item))
      );
  }

  saveStoreItem(item: StoreItem): Observable<StoreItem> {
    const command = item.id ? 'modify-store-item' : 'create-store-item';
    return this.http.post(`${this.apiUrl}/${apiRoute}/${command}`, item)
      .pipe(
        map((data: any) => {
          if (data?.id === undefined) {
            return item;
          }
          return this.adapter.adapt({
            id: data.id,
            code: item.code,
            data: item.data
          });
        })
      );
  }
}
