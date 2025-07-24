/* eslint-disable @typescript-eslint/no-explicit-any */
import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { map } from 'rxjs/operators';
import { dateToDateOnly, isDateOnlyString } from '@app/shared/helpers';
import { LoggerService } from '@app/core/logger';
import { tap } from 'rxjs';

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
  logger.info(`🚀 HTTP Request starting: ${req.method} ${req.url}`);

  return next(req).pipe(
    tap({
      next: (event) => {
        if (event instanceof HttpResponse) {
          const elapsed = Date.now() - startTime;
          logger.info(
            `📨 HTTP Response received: ${req.method} ${req.url} response received in ${elapsed}ms`,
            { status: event.status, elapsed }
          );
        }
      },
      error: (error) => {
        const elapsed = Date.now() - startTime;
        logger.error(
          `❌ HTTP Request failed: ${req.method} ${req.url} failed in ${elapsed}ms`,
          { error, elapsed }
        );
      },
      complete: () => {
        const elapsed = Date.now() - startTime;
        logger.info(
          `✅ HTTP Request completed: ${req.method} ${req.url} completed in ${elapsed}ms`,
          { elapsed }
        );
      },
    })
  );
};
