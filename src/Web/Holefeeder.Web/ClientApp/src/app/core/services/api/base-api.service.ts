import {HttpResponse} from '@angular/common/http';
import {PagingInfo} from '@app/core/models/paging-info.model';
import {Adapter} from '@app/shared/interfaces/adapter.interface';
import {throwError} from 'rxjs';

export abstract class BaseApiService {
  mapToPagingInfo<T>(resp: HttpResponse<Object[]>, adapter: Adapter<T>): PagingInfo<T> {
    const totalCount = +(resp.headers.get('X-Total-Count') ?? resp.body?.length ?? 0);
    return new PagingInfo<T>(totalCount, resp.body?.map(adapter.adapt) ?? []);
  }

  protected formatErrors(error: any) {
    console.error(error);
    return throwError(() => new Error(error.error));
  }
}
