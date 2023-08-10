/* eslint-disable @typescript-eslint/no-explicit-any */
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { dateToDateOnly, isDateOnlyString } from '@app/shared/helpers';

export class JsonDateOnlyInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const newReq = req.clone({
      body: this.convertOutgoing(req.body),
    });
    return next.handle(newReq).pipe(
      map((val: HttpEvent<any>) => {
        if (val instanceof HttpResponse) {
          const body = val.body;
          this.convertIncoming(body);
        }
        return val;
      })
    );
  }

  convertIncoming(body: any) {
    if (body === null || body === undefined) {
      return body;
    }
    if (typeof body !== 'object') {
      return body;
    }
    for (const key of Object.keys(body)) {
      const value = body[key];
      if (isDateOnlyString(value)) {
        body[key] = new Date(value);
      } else if (typeof value === 'object') {
        this.convertIncoming(value);
      }
    }
  }

  convertOutgoing(body: any) {
    if (body === null || body === undefined) {
      return body;
    }
    if (typeof body !== 'object') {
      return body;
    }
    for (const key of Object.keys(body)) {
      const value = body[key];
      if (value instanceof Date) {
        body[key] = dateToDateOnly(value);
      } else if (typeof value === 'object') {
        this.convertOutgoing(value);
      }
    }
  }
}
