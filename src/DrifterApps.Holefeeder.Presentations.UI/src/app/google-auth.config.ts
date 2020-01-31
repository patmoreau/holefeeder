import { AuthConfig } from 'angular-oauth2-oidc';

export const googleAuthConfig: AuthConfig = {
    // Url of the Identity Provider
    issuer: 'https://accounts.google.com',

    // URL of the SPA to redirect the user to after login
    redirectUri: window.location.origin + '/index.html',

    // URL of the SPA to redirect the user after silent refresh
    silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',

    // The SPA's id. The SPA is registerd with this id at the auth-server
    clientId: '890828371465-fdjn2okrids500dqcogjkogbca2i2929.apps.googleusercontent.com',
    strictDiscoveryDocumentValidation: false,

    // set the scope for the permissions the client should request
    // The first three are defined by OIDC. The 4th is a usecase-specific one
    scope: 'openid profile email',
    showDebugInformation: true,
    sessionChecksEnabled: true
};
