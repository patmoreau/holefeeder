import { BrowserModule } from '@angular/platform-browser';
import {NgModule, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER} from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastNoAnimationModule } from 'ngx-toastr';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { ErrorNotfoundComponent } from './error-notfound/error-notfound.component';
import { SingletonsModule } from './singletons/singletons.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthenticationService } from './auth/services/authentication.service';
import { AuthModule } from './auth/auth.module';
import { AuthModule as OAuthModule, OidcConfigService } from 'angular-auth-oidc-client';
import { OAuthInterceptor } from './auth/oauth.interceptor';
import { configureGoogleAuth } from './auth/google-auth.config';
import { AuthGuardService } from './auth/services/auth-guard.service';

const COMPONENTS = [
  AppComponent,
  HeaderComponent,
  FooterComponent,
  ErrorNotfoundComponent
];


@NgModule({
  declarations: [COMPONENTS],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule,
    OAuthModule.forRoot(),
    HttpClientModule,
    FontAwesomeModule,
    NgbModule,
    SingletonsModule,
    AuthModule,
    ToastNoAnimationModule.forRoot()
  ],
  providers: [
    OidcConfigService,
    {
        provide: APP_INITIALIZER,
        useFactory: configureGoogleAuth,
        deps: [OidcConfigService],
        multi: true,
    },
    // AuthenticationService,
    // AuthGuardService,
    { provide: HTTP_INTERCEPTORS, useClass: OAuthInterceptor, multi: true }
],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
