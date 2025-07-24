import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  enableProdMode,
  ErrorHandler,
  importProvidersFrom,
  isDevMode,
} from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { provideServiceWorker } from '@angular/service-worker';
import { loadConfigProvider } from '@app/app-initializer';
import { AppComponent } from '@app/app.component';
import { GlobalErrorHandler } from '@app/core/errors';
import {
  jsonDateOnlyInterceptor,
  httpRequestLoggerInterceptor
} from '@app/core/interceptors/functional-interceptors';
import { appEffects, appStore } from '@app/core/store';
import { ROUTES } from '@app/routes';
import { environment } from '@env/environment';
import { provideEffects } from '@ngrx/effects';
import { provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import {
  AbstractSecurityStorage,
  authInterceptor,
  AuthModule,
  DefaultLocalStorageService,
  LogLevel,
} from 'angular-auth-oidc-client';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

if (environment.production) {
  enableProdMode();
}

// Enhanced error handling for better debugging
function setupGlobalErrorHandling() {
  // Handle unhandled promise rejections
  window.addEventListener('unhandledrejection', (event) => {
    if (isDevMode()) {
      console.group('🚨 Unhandled Promise Rejection');
      console.error('Unhandled promise rejection:', event.reason);
      console.error('Promise:', event.promise);
      console.groupEnd();
    }

    // Prevent the error from appearing in console (optional)
    // event.preventDefault();
  });

  // Handle global JavaScript errors
  window.addEventListener('error', (event) => {
    if (isDevMode()) {
      console.group('🚨 Global JavaScript Error');
      console.error('Global error caught:', event.error);
      console.error('Message:', event.message);
      console.error('Source:', event.filename, 'Line:', event.lineno, 'Column:', event.colno);
      console.groupEnd();
    }
  });
}

// Set up error handling before bootstrapping
setupGlobalErrorHandling();

bootstrapApplication(AppComponent, {
  providers: [
    {
      provide: 'BASE_API_URL',
      useValue: `${environment.baseUrl}/gateway/api/v2`,
    },
    loadConfigProvider,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    provideHttpClient(
      withInterceptors([
        authInterceptor(),
        jsonDateOnlyInterceptor,
        httpRequestLoggerInterceptor,
      ])
    ),
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
          // Configure secureRoutes to match your API URLs
          secureRoutes: [`${environment.baseUrl}/gateway/api/v2`],
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
    {
      provide: AbstractSecurityStorage,
      useClass: DefaultLocalStorageService,
    },
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000',
    }),
  ],
})
  .then(() => {
    if (isDevMode()) {
      console.log('✅ Application bootstrap successful');
    }
  })
  .catch(err => {
    console.error('❌ Application bootstrap failed:', err);

    // In development, show more detailed error information
    if (isDevMode()) {
      console.group('📋 Bootstrap Error Details');
      console.error('Error:', err);
      console.error('Stack:', err.stack);
      console.groupEnd();

      // Optionally display error to user
      const errorContainer = document.createElement('div');
      errorContainer.style.padding = '20px';
      errorContainer.style.background = '#f8d7da';
      errorContainer.style.color = '#721c24';
      errorContainer.style.border = '1px solid #f5c6cb';
      errorContainer.style.borderRadius = '4px';
      errorContainer.style.margin = '20px';
      errorContainer.style.fontFamily = 'Arial, sans-serif';

      const errorTitle = document.createElement('h3');
      errorTitle.textContent = 'Application Failed to Start';
      errorContainer.appendChild(errorTitle);

      const errorMessage = document.createElement('p');
      errorMessage.innerHTML = '<strong>Error:</strong> ';
      const errorText = document.createTextNode(err.message);
      errorMessage.appendChild(errorText);
      errorContainer.appendChild(errorMessage);

      const errorDetails = document.createElement('details');
      const errorSummary = document.createElement('summary');
      errorSummary.textContent = 'Technical Details (Click to expand)';
      errorDetails.appendChild(errorSummary);

      const errorStack = document.createElement('pre');
      errorStack.style.background = '#f1f1f1';
      errorStack.style.padding = '10px';
      errorStack.style.margin = '10px 0';
      errorStack.style.overflow = 'auto';
      errorStack.textContent = err.stack;
      errorDetails.appendChild(errorStack);
      errorContainer.appendChild(errorDetails);

      const errorFooter = document.createElement('p');
      errorFooter.textContent = 'Please check the browser console for more information and contact support if the issue persists.';
      errorContainer.appendChild(errorFooter);

      document.body.textContent = ''; // Clear existing content
      document.body.appendChild(errorContainer);
    }
  });
