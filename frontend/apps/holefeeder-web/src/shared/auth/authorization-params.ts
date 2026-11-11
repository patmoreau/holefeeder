const scope = 'openid profile email read:user write:user';

const create = (redirectUri: string, audience: string) => ({
  redirect_uri: redirectUri,
  audience: audience,
  scope: scope,
});

export const AuthorizationParams = { create: create };
