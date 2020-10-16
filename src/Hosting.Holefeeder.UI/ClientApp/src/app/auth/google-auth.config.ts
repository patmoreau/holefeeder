import { OidcConfigService, LogLevel } from 'angular-auth-oidc-client';

export function configureGoogleAuth(oidcConfigService: OidcConfigService) {
    return () =>
      oidcConfigService.withConfig({
          stsServer: 'https://accounts.google.com/',
          redirectUrl: window.location.origin + '/oauthcallback',
          clientId: '890828371465-fdjn2okrids500dqcogjkogbca2i2929.apps.googleusercontent.com',
          responseType: 'id_token token',
          scope: 'openid email profile',
          triggerAuthorizationResultEvent: true,
          postLogoutRedirectUri: window.location.origin + '/login',
          startCheckSession: false,
          silentRenew: true,
          silentRenewUrl: window.location.origin + '/silent-renew.html',
          postLoginRoute: '/dashboard',
          forbiddenRoute: '/forbidden',
          unauthorizedRoute: '/unauthorized',
          logLevel: LogLevel.Error,
          historyCleanupOff: true,
      });
  }
  