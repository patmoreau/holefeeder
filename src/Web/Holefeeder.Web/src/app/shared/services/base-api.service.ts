import { HttpResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { Adapter } from '../interfaces/adapter.interface';
import { PagingInfo } from '../models/paging-info.model';

export abstract class BaseApiService {
  mapToPagingInfo<T>(resp: HttpResponse<Object[]>, adapter: Adapter<T>): PagingInfo<T> {
    return resp.headers.has('X-Total-Count')
      ? new PagingInfo<T>(+resp.headers.get('X-Total-Count'), resp.body.map(adapter.adapt))
      : new PagingInfo<T>(resp.body.length, resp.body.map(adapter.adapt));
  }

  protected formatErrors(error: any) {
    console.error(error);
    return throwError(() => new Error(error.error));
  }
}
