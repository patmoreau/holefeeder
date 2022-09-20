import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, retry, throwError } from 'rxjs';

const retryCount = 3;
const retryWaitMilliSeconds = 5000;

@Injectable({ providedIn: 'root' })
export class HttpLoadingInterceptor implements HttpInterceptor {
  constructor() {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
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
    ) as Observable<HttpEvent<any>>;
  }
}
