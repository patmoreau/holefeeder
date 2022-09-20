import { HttpResponse } from '@angular/common/http';
import { Adapter, PagingInfo } from '@app/shared/models';
import { throwError } from 'rxjs';

export function mapToPagingInfo<T>(
  resp: HttpResponse<Object[]>,
  adapter: Adapter<T>
): PagingInfo<T> {
  const totalCount = +(
    resp.headers.get('X-Total-Count') ??
    resp.body?.length ??
    0
  );
  return new PagingInfo<T>(totalCount, resp.body?.map(adapter.adapt) ?? []);
}

export function formatErrors(error: any) {
  return throwError(() => new Error(error.error));
}
