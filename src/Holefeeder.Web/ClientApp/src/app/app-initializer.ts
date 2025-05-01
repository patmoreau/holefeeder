import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { APP_INITIALIZER, FactoryProvider } from '@angular/core';
import { ConfigService } from '@app/core/services';
import { environment } from '@env/environment';
import { catchError, map, Observable, timeout, retry } from 'rxjs';
import { AngularSettings } from './shared/models';
import { LoggingLevel } from '@app/shared/models/logging-level.enum';

function loadConfigFactory(
  http: HttpClient,
  config: ConfigService
): () => Observable<boolean> {
  return () =>
    http.get<AngularSettings>(`${environment.baseUrl}/config`).pipe(
      timeout(10000), // 10 second timeout
      retry({ count: 2, delay: 1000 }), // Retry twice with 1s delay
      map((c: AngularSettings) => {
        config.setLoggingLevel(c.loggingLevel);
        return true;
      }),
      catchError((err: HttpErrorResponse) => {
        config.setLoggingLevel(LoggingLevel.None);
        console.error('Failed to load config:', err);
        // Return true to allow app to continue loading even if config fails
        return Promise.resolve(true);
      })
    );
}

export const loadConfigProvider: FactoryProvider = {
  provide: APP_INITIALIZER,
  useFactory: loadConfigFactory,
  deps: [HttpClient, ConfigService],
  multi: true,
};
