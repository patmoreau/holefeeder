import {BrowserModule} from '@angular/platform-browser';
import {NgModule, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER} from '@angular/core';
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
import {MsalModule, MSAL_CONFIG, MSAL_CONFIG_ANGULAR, MsalService} from '@azure/msal-angular';
import {AppConfigService} from "@app/app-config.service";
import {AnonymousMsalInterceptor} from "@app/interceptors/anonymous-msal.interceptor";

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent
];

const MSALConfigFactory = (appConfig: AppConfigService) => {
  return () => {
    console.log('MSALConfigFactory')
    return appConfig.getMsalConfig();
  };
}

const MSALAngularConfigFactory = (appConfig: AppConfigService) => {
  return () => {
    console.log('MSALAngularConfigFactory')
    return appConfig.getMsalAngularConfig();
  };
}

const appInitializerFn = (appConfig: AppConfigService) => {
  return () => {
    return appConfig.loadAppConfig().toPromise();
  };
};

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
    AppConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      multi: true,
      deps: [AppConfigService]
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AnonymousMsalInterceptor,
      multi: true
    },
    {
      provide: MSAL_CONFIG,
      useFactory: MSALConfigFactory,
      deps: [AppConfigService]
    },
    {
      provide: MSAL_CONFIG_ANGULAR,
      useFactory: MSALAngularConfigFactory,
      deps: [AppConfigService]
    },
    MsalService
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
}
