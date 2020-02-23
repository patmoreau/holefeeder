import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastNoAnimationModule } from 'ngx-toastr';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { fas } from '@fortawesome/free-solid-svg-icons';
import { far } from '@fortawesome/free-regular-svg-icons';
import { fab } from '@fortawesome/free-brands-svg-icons';
library.add(fas, far, fab);

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { ErrorNotfoundComponent } from './error-notfound/error-notfound.component';
import { SingletonsModule } from './singletons/singletons.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OAuthModule } from 'angular-oauth2-oidc';
import { LoginComponent } from './auth/login/login.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { OAuthInterceptor } from './auth/oauth.interceptor';
import { environment } from '@env/environment';
import { AuthenticationService } from './auth/services/authentication.service';

const COMPONENTS = [
  AppComponent,
  LoginComponent,
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
    HttpClientModule,
    FontAwesomeModule,
    NgbModule,
    SingletonsModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: [environment.api_url],
        sendAccessToken: true
      }
    }),
    ToastNoAnimationModule.forRoot()
  ],
  providers: [
    AuthenticationService,
    { provide: HTTP_INTERCEPTORS, useClass: OAuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
