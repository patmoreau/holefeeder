import {Injectable} from "@angular/core";
import {HttpBackend, HttpClient, HttpHeaders} from "@angular/common/http";
import {map} from "rxjs/operators";
import {ConfigAdapter, Config} from "@app/core/config/config.model";
import {BrowserCacheLocation, Configuration, InteractionType} from "@azure/msal-browser";
import {MsalGuardConfiguration, MsalInterceptorConfiguration} from "@azure/msal-angular";
import {tap} from "rxjs";
import {StateService} from "@app/core/services/state.service";

const isIE = window.navigator.userAgent.indexOf("MSIE ") > -1 || window.navigator.userAgent.indexOf("Trident/") > -1;

export const b2cPolicies = {
  names: {
    signUpSignIn: "B2C_1A_signup_signin_drifterapps",
    forgotPassword: "B2C_1A_PasswordReset",
    editProfile: "B2C_1A_ProfileEdit"
  },
  authorities: {
    signUpSignIn: {
      authority: "https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_signup_signin_drifterapps"
    },
    forgotPassword: {
      authority: "https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_PasswordReset"
    },
    editProfile: {
      authority: "https://holefeeder.b2clogin.com/holefeeder.onmicrosoft.com/B2C_1A_ProfileEdit"
    }
  },
  authorityDomain: "holefeeder.b2clogin.com"
};

const apiConfig: { scopes: string[]; uri: string } = {
  scopes: ["https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user"],
  uri: "https://holefeeder.onmicrosoft.com/holefeeder.api"
};

const loginRequest: { scopes: string[] } = {scopes: ["openid", "profile"]};

const tokenRequest: { scopes: string[] } = {scopes: apiConfig.scopes};

interface ConfigState {
  config: Config;
  msalConfiguration: Configuration;
  msalInterceptorConfiguration: MsalInterceptorConfiguration;
  msalGuardConfiguration: MsalGuardConfiguration;
}

const initialState: ConfigState = {
  config: new Config('/', '/'),
  msalConfiguration: Object.assign({}),
  msalInterceptorConfiguration: Object.assign({}),
  msalGuardConfiguration: Object.assign({
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: [...apiConfig.scopes]
    }
  })
};

@Injectable({providedIn: "root"})
export class ConfigService extends StateService<ConfigState> {
  private http: HttpClient;

  constructor(private readonly httpHandler: HttpBackend, private adapter: ConfigAdapter) {
    super(initialState);

    this.http = new HttpClient(httpHandler);
    const headers = new HttpHeaders().set("Content-Type", "application/json; charset=utf-8");

    this.http.get('/config')
      .pipe(
        map((data: any) => data.map(this.adapter.adapt)),
        tap(console.log)
      )
      .subscribe(config => {
        const protectedResourceMap = new Map<string, Array<string>>();
        protectedResourceMap.set(this.state.config.apiUrl, apiConfig.scopes);

        this.setState({
          config: config,
          msalConfiguration: Object.assign({
            auth: {
              clientId: "9814ecda-b8db-4775-a361-714af29fe486",
              authority: b2cPolicies.authorities.signUpSignIn.authority,
              redirectUri: config.redirectUrl,
              postLogoutRedirectUri: config.redirectUrl,
              knownAuthorities: [b2cPolicies.authorityDomain]
            },
            cache: {
              cacheLocation: BrowserCacheLocation.LocalStorage,
              storeAuthStateInCookie: isIE // set to true for IE 11
            }
          }),
          msalInterceptorConfiguration: Object.assign({
            interactionType: InteractionType.Redirect,
            protectedResourceMap
          })
        });
      });
  }

  get config(): Config {
    return this.state.config;
  }

  get msalConfiguration(): Configuration {
    return this.state.msalConfiguration;
  }

  get msalInterceptorConfiguration(): MsalInterceptorConfiguration {
    return this.state.msalInterceptorConfiguration;
  }

  get msalGuardConfiguration(): MsalGuardConfiguration {
    return this.state.msalGuardConfiguration;
  }
}
