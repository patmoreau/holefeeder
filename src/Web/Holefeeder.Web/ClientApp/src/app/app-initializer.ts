import {catchError, map, Observable, tap, throwError} from "rxjs";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {ConfigService, LoggingLevel} from "@app/core/services/config.service";
import {APP_INITIALIZER, FactoryProvider} from "@angular/core";

function loadConfigFactory(http: HttpClient, config: ConfigService): () => Observable<boolean> {
  return () => http.get('/config')
    .pipe(
      tap(console.log),
      map(c => {
        config.loggingLevel = c.loggingLevel;
        return true;
      }),
      catchError((err: HttpErrorResponse) => {
        config.loggingLevel = LoggingLevel.None;
        return throwError(() => new Error(err.message));
      })
    );
}

export const loadConfigProvider: FactoryProvider = {
  provide: APP_INITIALIZER,
  useFactory: loadConfigFactory,
  deps: [HttpClient, ConfigService],
  multi: true
};
