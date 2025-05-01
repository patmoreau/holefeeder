import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, retry, throwError, timeout } from 'rxjs';

const retryCount = 1;
const retryWaitMilliSeconds = 1000;
const defaultTimeout = 30000;

@Injectable({ providedIn: 'root' })
export class HttpLoadingInterceptor implements HttpInterceptor {
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      timeout(defaultTimeout),
      retry({ count: retryCount, delay: retryWaitMilliSeconds }),
      catchError((err: HttpErrorResponse) => {
        let errorMessage = '';
        if (err.error instanceof ErrorEvent) {
          // client-side error
          errorMessage = `Error: ${err.message}`;
        } else {
          // server-side error
          errorMessage = `Error Code: ${err.status}\nMessage: ${err.message}`;
        }
        return throwError(() => new Error(errorMessage));
      })
    ) as Observable<HttpEvent<unknown>>;
  }
}
