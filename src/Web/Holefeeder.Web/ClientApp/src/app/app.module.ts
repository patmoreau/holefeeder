import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, ErrorHandler, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { loadConfigProvider } from '@app/app-initializer';
import { GlobalErrorHandler } from '@app/core/errors/global-error-handler';
import { HttpLoadingInterceptor } from '@app/core/errors/http-loading.interceptor';
import { ResourceNotfoundComponent } from '@app/core/resource-notfound/resource-notfound.component';
import { ExternalUrlDirective } from '@app/directives/external-url.directive';
import { environment } from '@env/environment';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { LoadingBarModule } from '@ngx-loading-bar/core';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { ToastNoAnimationModule } from 'ngx-toastr';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthConfigModule } from './auth-config.module';
import { ErrorNotfoundComponent } from './core/error-notfound/error-notfound.component';
import { FooterComponent } from './core/footer/footer.component';
import { HeaderComponent } from './core/header/header.component';
import { SharedModule } from './shared/shared.module';
import { AuthInterceptor } from 'angular-auth-oidc-client';

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent,
  ResourceNotfoundComponent,
  ExternalUrlDirective,
];

@NgModule({
  declarations: [COMPONENTS],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    AuthConfigModule,
    SharedModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    ToastNoAnimationModule.forRoot(),
    LoadingBarRouterModule,
    LoadingBarModule,
  ],
  providers: [
    { provide: 'BASE_API_URL', useValue: `${environment.baseUrl}/gateway` },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    // {
    //   provide: HTTP_INTERCEPTORS,
    //   useClass: HttpLoadingInterceptor,
    //   multi: true,
    // },
    loadConfigProvider,
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule {}
