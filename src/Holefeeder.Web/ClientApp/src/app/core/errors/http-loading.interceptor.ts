import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, retryWhen, throwError, timeout, tap, delay, take } from 'rxjs';

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
      retryWhen(errors =>
        errors.pipe(
          // Only retry on specific conditions: network errors, timeouts, or 5xx server errors
          tap(err => {
            if (!(err.error instanceof ErrorEvent) &&
              err.name !== 'TimeoutError' &&
              !(err.status >= 500 && err.status < 600)) {
              throw err; // Don't retry for 4xx errors or other non-retryable errors
            }
          }),
          delay(retryWaitMilliSeconds),
          take(retryCount)
        )
      ),
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
