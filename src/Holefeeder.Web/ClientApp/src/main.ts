import { HTTP_INTERCEPTORS } from '@angular/common/http';
import {
  enableProdMode,
  ErrorHandler,
  importProvidersFrom,
  isDevMode,
} from '@angular/core';
import '@angular/localize/init';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { provideServiceWorker } from '@angular/service-worker';
import { loadConfigProvider } from '@app/app-initializer';
import { AppComponent } from '@app/app.component';
import { GlobalErrorHandler, HttpLoadingInterceptor } from '@app/core/errors';
import { HttpRequestLoggerInterceptor } from '@app/core/interceptors/http-request-logger.interceptor';
import { JsonDateOnlyInterceptor } from '@app/core/interceptors/json-dateonly-interceptor';
import { appEffects, appStore } from '@app/core/store';
import { ROUTES } from '@app/routes';
import { environment } from '@env/environment';
import { provideEffects } from '@ngrx/effects';
import { provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import {
  AbstractSecurityStorage,
  AuthInterceptor,
  AuthModule,
  DefaultLocalStorageService,
  LogLevel,
} from 'angular-auth-oidc-client';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: 'BASE_API_URL',
      useValue: `${environment.baseUrl}/gateway/api/v2`,
    },
    loadConfigProvider,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    importProvidersFrom(
      RouterModule.forRoot(ROUTES, {
        enableTracing: !environment.production && environment.enableTracing,
      }),
      AuthModule.forRoot({
        config: {
          authority: 'https://dev-vx1jio3owhaqdmqa.ca.auth0.com',
          clientId: 'gXVZOUw6cxoIR6N5qqXFyRVuZDbkkVxu',
          redirectUrl: window.location.origin,
          postLogoutRedirectUri: window.location.origin,
          responseType: 'code',
          scope: 'openid profile email offline_access read:user write:user',
          silentRenew: true,
          useRefreshToken: true,
          logLevel: LogLevel.Warn,
          postLoginRoute: '/dashboard',
          customParamsAuthRequest: {
            audience: 'https://holefeeder-api.drifterapps.app',
          },
          secureRoutes: [
            'https://holefeeder.localtest.me',
            'http://localhost:3000',
            '/gateway/api',
          ],
        },
      })
    ),
    provideCharts(withDefaultRegisterables()),
    provideStore(appStore),
    provideEffects(appEffects),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
      trace: false,
      traceLimit: 75, // maximum stack trace frames to be stored (in case trace option was provided as true)
      connectInZone: true,
    }),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JsonDateOnlyInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpRequestLoggerInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
    {
      provide: AbstractSecurityStorage,
      useClass: DefaultLocalStorageService,
    },
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000',
    }),
  ],
}).catch(err => console.error(err));
