import {BrowserModule} from '@angular/platform-browser';
import {NgModule, CUSTOM_ELEMENTS_SCHEMA, ErrorHandler} from '@angular/core';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {ToastNoAnimationModule} from 'ngx-toastr';
import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {HeaderComponent} from './core/header/header.component';
import {FooterComponent} from './core/footer/footer.component';
import {ErrorNotfoundComponent} from './core/error-notfound/error-notfound.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ReactiveFormsModule, FormsModule} from '@angular/forms';
import {
  MsalModule,
  MsalService,
  MsalBroadcastService,
  MsalGuard,
  MsalRedirectComponent
} from '@azure/msal-angular';
import {RedirectComponent} from "@app/redirect.component";
import {ExternalUrlDirective} from "@app/directives/external-url.directive";
import {LoadingBarRouterModule} from '@ngx-loading-bar/router';
import {LoadingBarModule} from '@ngx-loading-bar/core';
import {ResourceNotfoundComponent} from "@app/core/resource-notfound/resource-notfound.component";
import {SharedModule} from './shared/shared.module';
import {loadConfigProvider} from "@app/app-initializer";
import {
  apiConfig,
  b2cPolicies, isIE, loggerCallback,
  msalGuardConfigProvider,
  msalInstanceProvider,
  msalInterceptorConfigProvider,
  msalInterceptorProvider
} from "@app/app-msal";
import {GlobalErrorHandler} from "@app/core/errors/global-error-handler";
import {HttpLoadingInterceptor} from "@app/core/errors/http-loading.interceptor";
import {environment} from "@env/environment";
import {BrowserCacheLocation, InteractionType, LogLevel, PublicClientApplication} from "@azure/msal-browser";

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent,
  ResourceNotfoundComponent,
  RedirectComponent,
  ExternalUrlDirective
];

@NgModule({
  declarations: [COMPONENTS],
  imports: [
    BrowserModule,//.withServerTransition({appId: 'ng-cli-universal'}),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    MsalModule,
    SharedModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    ToastNoAnimationModule.forRoot(),
    LoadingBarRouterModule,
    LoadingBarModule
  ],
  providers: [
    { provide: "BASE_API_URL", useValue: '/gateway' },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpLoadingInterceptor,
      multi: true,
    },
    loadConfigProvider,
    msalInstanceProvider,
    msalGuardConfigProvider,
    msalInterceptorConfigProvider,
    msalInterceptorProvider,
    MsalService,
    MsalGuard,
    MsalBroadcastService
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
}
