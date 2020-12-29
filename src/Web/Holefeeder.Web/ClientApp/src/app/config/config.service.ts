import {Injectable} from '@angular/core';
import {HttpBackend, HttpClient, HttpHeaders} from '@angular/common/http';
import {Configuration} from "msal";
import {MsalAngularConfiguration} from "@azure/msal-angular";
import {BehaviorSubject, Observable} from "rxjs";
import {map, tap} from "rxjs/operators";
import {IConfig} from "@app/config/config.interface";

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

@Injectable({
    providedIn: 'root'
  }
)
export class ConfigService {
  private configData: IConfig;

  private http: HttpClient;

  constructor(private readonly httpHandler: HttpBackend) {
    this.http = new HttpClient(httpHandler);
  }

  init(endpoint: string): Promise<boolean> {
    return new Promise<boolean>((resolve, reject) => {
      this.http.get<IConfig>(endpoint).pipe(map(res => res))
        .subscribe(value => {
            this.configData = Object.assign(value);
            resolve(true);
          },
          (error) => {
            reject(error);
          })
    });
  }

  get config(): IConfig {
    return this.configData;
  }

  getMsalConfig(): Configuration {
    return Object.assign({
      auth: {
        clientId: '9814ecda-b8db-4775-a361-714af29fe486',
        authority: b2cPolicies.authorities.signUpSignIn.authority,
        redirectUri: this.configData.redirectUrl,
        postLogoutRedirectUri: this.configData.redirectUrl,
        navigateToLoginRequestUrl: true,
        validateAuthority: false,
      },
      cache: {
        cacheLocation: 'localStorage',
        storeAuthStateInCookie: isIE,
      },
    });
  }

  getMsalAngularConfig(): MsalAngularConfiguration {
    return Object.assign({
      popUp: !isIE,
      consentScopes: [
        ...loginRequest.scopes,
        ...tokenRequest.scopes,
      ],
      unprotectedResources: [],
      protectedResourceMap: [[this.configData.apiUrl, apiConfig.b2cScopes]],
      extraQueryParameters: {}
    });
  }
}
