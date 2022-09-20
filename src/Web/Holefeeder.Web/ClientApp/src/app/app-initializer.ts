import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { APP_INITIALIZER, FactoryProvider } from '@angular/core';
import { LoggingLevel } from '@app/core';
import { ConfigService } from '@app/core/services/config.service';
import { environment } from '@env/environment';
import { catchError, map, Observable, throwError } from 'rxjs';

function loadConfigFactory(
  http: HttpClient,
  config: ConfigService
): () => Observable<boolean> {
  return () =>
    http.get(`${environment.baseUrl}/config`).pipe(
      map((c: any) => {
        config.setLoggingLevel(c.loggingLevel);
        return true;
      }),
      catchError((err: HttpErrorResponse) => {
        config.setLoggingLevel(LoggingLevel.None);
        return throwError(() => new Error(err.message));
      })
    );
}

export const loadConfigProvider: FactoryProvider = {
  provide: APP_INITIALIZER,
  useFactory: loadConfigFactory,
  deps: [HttpClient, ConfigService],
  multi: true,
};
