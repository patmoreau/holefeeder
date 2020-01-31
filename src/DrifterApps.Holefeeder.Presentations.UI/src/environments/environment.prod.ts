export const environment = {
  production: true,
  api_url: 'https://holefeeder-api.drifterapps.com/',
  oktaConfig: {
    issuer: 'https://dev-412843.oktapreview.com/oauth2/default',
    redirectUri: 'https://holefeeder.drifterapps.com/implicit/callback',
    clientId: '0oajk4y3awa4DyTID0h7'
  },
  jwtConfig: {
    whitelistedDomains: [
      'holefeeder-api.drifterapps.com'
    ],
    blacklistedRoutes: []
  }
};
