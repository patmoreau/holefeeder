import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {IAppConfig} from "@app/interfaces/app-config.interface";
import {Configuration} from "msal";
import {MsalAngularConfiguration} from "@azure/msal-angular";
import {Observable} from "rxjs";
import {tap} from "rxjs/operators";

const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

export const b2cPolicies = {
  names: {
    signUpSignIn: 'B2C_1A_signup_signin_drifterapps',
    resetPassword: 'B2C_1A_PasswordReset',
    editProfile: 'B2C_1A_ProfileEdit'
  },
  authorities: {
    signUpSignIn: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_signup_signin_drifterapps'
    },
    resetPassword: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_PasswordReset'
    },
    editProfile: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_ProfileEdit'
    }
  }
};

const apiConfig: { b2cScopes: string[]; webApi: string } = {
  b2cScopes: ['https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user'],
  webApi: 'https://holefeeder.onmicrosoft.com/holefeeder.api'
};

const loginRequest: { scopes: string[] } = {scopes: ['openid', 'profile']};

const tokenRequest: { scopes: string[] } = {scopes: apiConfig.b2cScopes};

@Injectable()
export class AppConfigService {
  private appConfig: IAppConfig;

  constructor(private http: HttpClient) {
  }

  loadAppConfig(): Observable<IAppConfig> {
    console.log('getting settings');
    const headers = new HttpHeaders({'Anonymous': ''})
    return this.http.get<IAppConfig>('/assets/Settings', {headers: headers})
      .pipe(
        tap(data => {
          console.log(data);
          this.appConfig = data;
        })
      );
  }

  getConfig(): IAppConfig {
    return this.appConfig;
  }

  getMsalConfig(): Configuration {
    return {
      auth: {
        clientId: '9814ecda-b8db-4775-a361-714af29fe486',
        authority: b2cPolicies.authorities.signUpSignIn.authority,
        redirectUri: this.appConfig.RedirectUrl,
        postLogoutRedirectUri: this.appConfig.RedirectUrl,
        navigateToLoginRequestUrl: true,
        validateAuthority: false,
      },
      cache: {
        cacheLocation: 'localStorage',
        storeAuthStateInCookie: isIE,
      },
    };
  }

  getMsalAngularConfig(): MsalAngularConfiguration {
    return {
      popUp: false,
      consentScopes: [
        ...loginRequest.scopes,
        ...tokenRequest.scopes,
      ],
      unprotectedResources: [],
      protectedResourceMap: [[this.appConfig.ApiUrl, apiConfig.b2cScopes]],
      extraQueryParameters: {}
    };
  }
}
