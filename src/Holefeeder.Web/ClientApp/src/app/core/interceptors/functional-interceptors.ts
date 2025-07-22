/* eslint-disable @typescript-eslint/no-explicit-any */
import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { map } from 'rxjs/operators';
import { dateToDateOnly, isDateOnlyString } from '@app/shared/helpers';
import { LoggerService } from '@app/core/logger';
import { catchError, retry, throwError, timeout, tap } from 'rxjs';

// JsonDateOnly Interceptor (functional)
export const jsonDateOnlyInterceptor: HttpInterceptorFn = (req, next) => {
  const newReq = req.clone({
    body: convertOutgoing(req.body),
  });

  return next(newReq).pipe(
    map((event) => {
      if (event instanceof HttpResponse) {
        const body = event.body;
        convertIncoming(body);
      }
      return event;
    })
  );
};

function convertIncoming(body: any) {
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
      convertIncoming(value);
    }
  }
}

function convertOutgoing(body: any) {
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
      convertOutgoing(value);
    }
  }
}

// HttpRequestLogger Interceptor (functional)
export const httpRequestLoggerInterceptor: HttpInterceptorFn = (req, next) => {
  const logger = inject(LoggerService);

  // Only log non-static requests
  if (req.url.includes('/assets/') || req.url.includes('/config')) {
    return next(req);
  }

  const startTime = Date.now();
  return next(req).pipe(
    tap({
      next: (event) => {
        if (event instanceof HttpResponse) {
          const elapsed = Date.now() - startTime;
          logger.info(
            `${req.method} ${req.url} completed in ${elapsed}ms`,
            { status: event.status, elapsed }
          );
        }
      },
      error: (error) => {
        const elapsed = Date.now() - startTime;
        logger.error(
          `${req.method} ${req.url} failed in ${elapsed}ms`,
          { error, elapsed }
        );
      }
    })
  );
};

// HttpLoading Interceptor (functional)
export const httpLoadingInterceptor: HttpInterceptorFn = (req, next) => {
  const retryCount = 1;
  const retryWaitMilliSeconds = 1000;
  const defaultTimeout = 30000;

  return next(req).pipe(
    timeout(defaultTimeout),
    retry({ count: retryCount, delay: retryWaitMilliSeconds }),
    catchError((err: any) => {
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
  );
};
