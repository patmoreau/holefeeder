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
import { loadConfigProvider } from '@app/app-initializer';
import { AppComponent } from '@app/app.component';
import { GlobalErrorHandler, HttpLoadingInterceptor } from '@app/core/errors';
import { ROUTES } from '@app/routes';
import { environment } from '@env/environment';
import {
  AbstractSecurityStorage,
  AuthInterceptor,
  AuthModule,
  DefaultLocalStorageService,
  LogLevel,
} from 'angular-auth-oidc-client';
import { JsonDateOnlyInterceptor } from '@app/core/interceptors/json-dateonly-interceptor';
import { provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { appEffects, appStore } from '@app/core/store';
import { provideEffects } from '@ngrx/effects';
import { HttpRequestLoggerInterceptor } from '@app/core/interceptors/http-request-logger.interceptor';
import { provideServiceWorker } from '@angular/service-worker';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    { provide: 'BASE_API_URL', useValue: `${environment.baseUrl}/gateway/api/v2` },
    loadConfigProvider,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    importProvidersFrom(
      RouterModule.forRoot(ROUTES, {
        enableTracing: !environment.production && environment.enableTracing,
      }),
      AuthModule.forRoot({
        config: {
          authority:
            'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_signup_signin_drifterapps/v2.0',
          authWellknownEndpointUrl:
            'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_signup_signin_drifterapps/v2.0/.well-known/openid-configuration',
          redirectUrl: window.location.origin,
          postLogoutRedirectUri: window.location.origin,
          clientId: '9814ecda-b8db-4775-a361-714af29fe486',
          scope:
            'openid profile offline_access https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user',
          responseType: 'code',
          silentRenew: true,
          silentRenewUrl: `${window.location.origin}/silent-renew.html`,
          useRefreshToken: true,
          triggerAuthorizationResultEvent: false,
          renewTimeBeforeTokenExpiresInSeconds: 30,
          ignoreNonceAfterRefresh: true,
          maxIdTokenIatOffsetAllowedInSeconds: 600,
          issValidationOff: false,
          autoUserInfo: false,
          logLevel: LogLevel.Debug,
          customParamsAuthRequest: {
            prompt: 'select_account', // login, consent
          },
          postLoginRoute: '/dashboard',
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
      connectInZone: true}),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
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
