import {BrowserModule} from '@angular/platform-browser';
import {NgModule, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER, InjectionToken} from '@angular/core';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {ToastNoAnimationModule} from 'ngx-toastr';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http';
import {HeaderComponent} from './header/header.component';
import {FooterComponent} from './footer/footer.component';
import {ErrorNotfoundComponent} from './error-notfound/error-notfound.component';
import {SingletonsModule} from './singletons/singletons.module';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ReactiveFormsModule, FormsModule} from '@angular/forms';
import {
  MsalModule,
  MsalService,
  MsalInterceptor,
  MsalGuardConfiguration,
  MsalInterceptorConfiguration,
  MSAL_INSTANCE,
  MsalBroadcastService,
  MsalGuard,
  MSAL_GUARD_CONFIG, MSAL_INTERCEPTOR_CONFIG
} from '@azure/msal-angular';
import {ConfigService} from "@app/config/config.service";
import {
  IPublicClientApplication,
  PublicClientApplication
} from "@azure/msal-browser";
import {RedirectComponent} from "@app/redirect.component";
import {ExternalUrlDirective} from "@app/directives/external-url.directive";

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent,
  RedirectComponent,
  ExternalUrlDirective
];

const AUTH_CONFIG_URL_TOKEN = new InjectionToken<string>('/assets/config');

export function initializerFactory(config: ConfigService, configUrl: string): any {
  const promise = config.loadAppConfig(configUrl).then(_ => {
    console.debug('Configuration initialized.');
  });
  return () => promise;
}

export function MSALInstanceFactory(config: ConfigService): IPublicClientApplication {
  return new PublicClientApplication(config.msalConfiguration);
}

export function MSALInterceptorConfigFactory(config: ConfigService): MsalInterceptorConfiguration {
  return config.msalInterceptorConfiguration;
}

export function MSALGuardConfigFactory(config: ConfigService): MsalGuardConfiguration {
  return config.msalGuardConfiguration;
}

@NgModule({
  declarations: [COMPONENTS],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    MsalModule,
    AppRoutingModule,
    HttpClientModule,
    FontAwesomeModule,
    NgbModule,
    SingletonsModule,
    ToastNoAnimationModule.forRoot()
  ],
  providers: [
    ConfigService,
    {provide: AUTH_CONFIG_URL_TOKEN, useValue: '/assets/config'},
    {
      provide: APP_INITIALIZER, useFactory: initializerFactory,
      deps: [ConfigService, AUTH_CONFIG_URL_TOKEN], multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },
    {
      provide: MSAL_INSTANCE,
      useFactory: MSALInstanceFactory,
      deps: [ConfigService]
    },
    {
      provide: MSAL_GUARD_CONFIG,
      useFactory: MSALGuardConfigFactory,
      deps: [ConfigService]
    },
    {
      provide: MSAL_INTERCEPTOR_CONFIG,
      useFactory: MSALInterceptorConfigFactory,
      deps: [ConfigService]
    },
    MsalService,
    MsalGuard,
    MsalBroadcastService
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
}
