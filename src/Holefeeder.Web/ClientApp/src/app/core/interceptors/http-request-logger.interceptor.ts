import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Injectable } from '@angular/core';
import { LoggerService } from '@app/core/logger';

@Injectable({ providedIn: 'root' })
export class HttpRequestLoggerInterceptor implements HttpInterceptor {
  constructor(private logger: LoggerService) {}
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    this.logger.info(`HttpRequestLoggerInterceptor - ${request.url}`, request);
    return next.handle(request).pipe(
      tap(event => {
        this.logger.info(
          `HttpRequestLoggerInterceptor - ${request.url} - ${event.type}`,
          event
        );
      })
    ) as Observable<HttpEvent<unknown>>;
  }
}
