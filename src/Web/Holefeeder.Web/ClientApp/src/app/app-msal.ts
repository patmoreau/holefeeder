import {
  BrowserCacheLocation,
  InteractionType,
  IPublicClientApplication,
  LogLevel,
  PublicClientApplication
} from "@azure/msal-browser";
import {
  MSAL_GUARD_CONFIG,
  MSAL_INSTANCE,
  MSAL_INTERCEPTOR_CONFIG,
  MsalGuardConfiguration,
  MsalInterceptor,
  MsalInterceptorConfiguration
} from "@azure/msal-angular";
import {ClassProvider, FactoryProvider} from "@angular/core";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {LoggerService} from "@app/core/logger/logger.service";

export const isIE = window.navigator.userAgent.indexOf("MSIE ") > -1 || window.navigator.userAgent.indexOf("Trident/") > -1;

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

export const apiConfig: { scopes: string[]; uri: string } = {
  scopes: ["https://holefeeder.onmicrosoft.com/holefeeder.api/holefeeder.user"],
  uri: "https://holefeeder.onmicrosoft.com/holefeeder.api"
};

export function MSALInstanceFactory(logger: LoggerService): IPublicClientApplication {
  function loggerCallback(logLevel: LogLevel, message: string) {
    if (logLevel === LogLevel.Error) {
      logger.logError(message);
    } else if (logLevel === LogLevel.Warning) {
      logger.logWarning(message);
    } else if (logLevel === LogLevel.Info) {
      logger.logInfo(message);
    } else {
      logger.logVerbose(message);
    }
  }

  return new PublicClientApplication({
    auth: {
      clientId: '9814ecda-b8db-4775-a361-714af29fe486',
      authority: b2cPolicies.authorities.signUpSignIn.authority,
      redirectUri: '/',
      postLogoutRedirectUri: '/',
      knownAuthorities: [b2cPolicies.authorityDomain]
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
      storeAuthStateInCookie: isIE, // set to true for IE 11
    },
    system: {
      loggerOptions: {
        loggerCallback,
        logLevel: LogLevel.Info,
        piiLoggingEnabled: false
      }
    }
  });
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
  return {
    interactionType: InteractionType.Redirect,
    protectedResourceMap: new Map([
      // ['https://graph.microsoft.com/v1.0/me', ['https://graph.microsoft.com/User.Read']],
      ['/gateway', [...apiConfig.scopes]],
    ])
  };
}

export function MSALGuardConfigFactory(): MsalGuardConfiguration {
  return Object.assign({
    interactionType: InteractionType.Redirect,
    authRequest: {
      scopes: [...apiConfig.scopes],
    },
  });
}

export const msalInterceptorProvider: ClassProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: MsalInterceptor,
  multi: true
};

export const msalInstanceProvider: FactoryProvider = {
  provide: MSAL_INSTANCE,
  useFactory: MSALInstanceFactory,
  deps: [LoggerService]
};

export const msalGuardConfigProvider: FactoryProvider = {
  provide: MSAL_GUARD_CONFIG,
  useFactory: MSALGuardConfigFactory
};

export const msalInterceptorConfigProvider: FactoryProvider = {
  provide: MSAL_INTERCEPTOR_CONFIG,
  useFactory: MSALInterceptorConfigFactory
};
