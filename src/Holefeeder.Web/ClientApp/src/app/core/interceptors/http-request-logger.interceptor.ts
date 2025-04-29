import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { LoggerService } from '@app/core/logger';

@Injectable({ providedIn: 'root' })
export class HttpRequestLoggerInterceptor implements HttpInterceptor {
  constructor(private logger: LoggerService) { }

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // Only log non-static requests
    if (request.url.includes('/assets/') || request.url.includes('/config')) {
      return next.handle(request);
    }

    const startTime = Date.now();
    return next.handle(request).pipe(
      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            const elapsed = Date.now() - startTime;
            this.logger.info(
              `${request.method} ${request.url} completed in ${elapsed}ms`,
              { status: event.status, elapsed }
            );
          }
        },
        error: (error) => {
          const elapsed = Date.now() - startTime;
          this.logger.error(
            `${request.method} ${request.url} failed in ${elapsed}ms`,
            { error, elapsed }
          );
        }
      })
    );
  }
}
