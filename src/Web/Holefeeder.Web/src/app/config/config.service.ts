import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient, HttpHeaders } from '@angular/common/http';
import { map, tap } from "rxjs/operators";
import { IConfig } from "@app/config/config.interface";
import { BrowserCacheLocation, Configuration, InteractionType } from "@azure/msal-browser";
import { MsalGuardConfiguration, MsalInterceptorConfiguration } from "@azure/msal-angular";
import { Observable } from 'rxjs';

const isIE = window.navigator.userAgent.indexOf('MSIE ') > -1 || window.navigator.userAgent.indexOf('Trident/') > -1;

export const b2cPolicies = {
  names: {
    signUpSignIn: 'B2C_1A_signup_signin_drifterapps',
    forgotPassword: 'B2C_1A_PasswordReset',
    editProfile: 'B2C_1A_ProfileEdit'
  },
  authorities: {
    signUpSignIn: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_signup_signin_drifterapps'
    },
    forgotPassword: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_PasswordReset'
    },
    editProfile: {
      authority: 'https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_ProfileEdit'
    }
  },
  authorityDomain: "holefeeder.b2clogin.com"
};

const apiConfig: { scopes: string[]; uri: string } = {
  scopes: ['https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user'],
  uri: 'https://holefeeder.onmicrosoft.com/holefeeder.api'
};

const loginRequest: { scopes: string[] } = { scopes: ['openid', 'profile'] };

const tokenRequest: { scopes: string[] } = { scopes: apiConfig.scopes };

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private configData: IConfig;

  private http: HttpClient;

  constructor(private readonly httpHandler: HttpBackend) {
    this.http = new HttpClient(httpHandler);
  }

  loadAppConfig(endpoint: string): Promise<boolean> {
    const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');

    return new Promise<boolean>((resolve, reject) => {
      this.http.get(`${endpoint}/_ngx-rtconfig.json?cb=${new Date().getTime()}`, { headers })
        .pipe(
          map(data => {
            let config: any = {};
            for (const key in data) {
              if (data.hasOwnProperty(key)) {
                config[key.replace('NGX_', '').toLowerCase().split('_').map((el, i) => (i > 0 ? el.charAt(0).toUpperCase() + el.slice(1) : el)).join('')] = data[key];
              }
            }
            return config;
          })
        )
        .subscribe(value => {
          this.configData = Object.assign(value);
          resolve(true);
        },
          (error) => {
            reject(error);
          });
    })
  }

  get config(): IConfig {
      return this.configData;
    }

  get msalConfiguration(): Configuration {
      return Object.assign({
        auth: {
          clientId: '9814ecda-b8db-4775-a361-714af29fe486',
          authority: b2cPolicies.authorities.signUpSignIn.authority,
          redirectUri: this.configData.redirectUrl,
          postLogoutRedirectUri: this.configData.redirectUrl,
          knownAuthorities: [b2cPolicies.authorityDomain]
        },
        cache: {
          cacheLocation: BrowserCacheLocation.LocalStorage,
          storeAuthStateInCookie: isIE, // set to true for IE 11
        },
      });
    }

  get msalInterceptorConfiguration(): MsalInterceptorConfiguration {
      const protectedResourceMap = new Map<string, Array<string>>();
      protectedResourceMap.set(this.configData.apiUrl, apiConfig.scopes);

      return Object.assign({
        interactionType: InteractionType.Redirect,
        protectedResourceMap,
      });
    }

  get msalGuardConfiguration(): MsalGuardConfiguration {
      return Object.assign({
        interactionType: InteractionType.Redirect,
        authRequest: {
          scopes: [...apiConfig.scopes],
        },
      });
    }
}
