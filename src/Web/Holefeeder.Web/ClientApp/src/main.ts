import { HTTP_INTERCEPTORS } from '@angular/common/http';
import {
  enableProdMode,
  ErrorHandler,
  importProvidersFrom,
} from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { loadConfigProvider } from '@app/app-initializer';
import { AppComponent } from '@app/app.component';
import { GlobalErrorHandler, HttpLoadingInterceptor } from '@app/core/errors';
import { ROUTES } from '@app/routes';
import { environment } from '@env/environment';
import {
  AuthInterceptor,
  AuthModule,
  LogLevel,
} from 'angular-auth-oidc-client';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    { provide: 'BASE_API_URL', useValue: `${environment.baseUrl}/gateway` },
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
    loadConfigProvider,
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
          useRefreshToken: true,
          ignoreNonceAfterRefresh: true,
          maxIdTokenIatOffsetAllowedInSeconds: 600,
          issValidationOff: false, // this needs to be true if using a common endpoint in Azure
          autoUserInfo: false,
          logLevel: LogLevel.Error,
          customParamsAuthRequest: {
            prompt: 'select_account', // login, consent
          },
          secureRoutes: [
            'https://holefeeder.test',
            'http://localhost:3000',
            '/gateway/api',
          ],
        },
      })
    ),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
}).catch(err => console.error(err));
