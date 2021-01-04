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
  MSAL_CONFIG,
  MSAL_CONFIG_ANGULAR,
  MsalService,
  MsalAngularConfiguration,
  MsalInterceptor
} from '@azure/msal-angular';
import {Configuration} from "msal";
import {ConfigService} from "@app/config/config.service";

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent
];

const AUTH_CONFIG_URL_TOKEN = new InjectionToken<string>('/assets/Settings');

export function initializerFactory(env: ConfigService, configUrl: string): any {
  // APP_INITIALIZER, except a function return which will return a promise
  // APP_INITIALIZER, angular doesnt starts application until it completes
  const promise = env.init(configUrl).then((value) => {
    console.debug('Configuration initialized.');
  });
  return () => promise;
}

export function msalConfigFactory(config: ConfigService): Configuration {
  return config.getMsalConfig();
}

export function msalAngularConfigFactory(config: ConfigService): MsalAngularConfiguration {
  return config.getMsalAngularConfig();
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
    {provide: AUTH_CONFIG_URL_TOKEN, useValue: '/assets/Settings'},
    {
      provide: APP_INITIALIZER, useFactory: initializerFactory,
      deps: [ConfigService, AUTH_CONFIG_URL_TOKEN], multi: true
    },
    {
      provide: MSAL_CONFIG,
      useFactory: msalConfigFactory,
      deps: [ConfigService]
    },
    {
      provide: MSAL_CONFIG_ANGULAR,
      useFactory: msalAngularConfigFactory,
      deps: [ConfigService]
    },
    MsalService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
}
