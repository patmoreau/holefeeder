import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER } from '@angular/core';
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
import { MsalModule, MsalInterceptor, MsalAngularConfiguration, MSAL_CONFIG, MSAL_CONFIG_ANGULAR, MsalService } from '@azure/msal-angular';
import { Configuration } from 'msal';
import { msalAngularConfig, msalConfig } from './app-config';

const COMPONENTS = [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ErrorNotfoundComponent
];

function MSALConfigFactory(): Configuration {
    return msalConfig;
}

function MSALAngularConfigFactory(): MsalAngularConfiguration {
    return msalAngularConfig;
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
        {
            provide: HTTP_INTERCEPTORS,
            useClass: MsalInterceptor,
            multi: true
        },
        {
            provide: MSAL_CONFIG,
            useFactory: MSALConfigFactory
        },
        {
            provide: MSAL_CONFIG_ANGULAR,
            useFactory: MSALAngularConfigFactory
        },
        MsalService
    ],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {
}
