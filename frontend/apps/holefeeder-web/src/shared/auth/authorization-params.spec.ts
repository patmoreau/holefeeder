import { describe, expect, it } from 'vitest';
import { AuthorizationParams } from './authorization-params';

describe('AuthorizationParams', () => {
  const params = AuthorizationParams.create('https://app.example', 'https://api.example');

  it('ShouldRequestTheApiAudience', () => {
    expect(params.audience).toBe('https://api.example');
  });

  it('ShouldRedirectBackToTheApp', () => {
    expect(params.redirect_uri).toBe('https://app.example');
  });

  it('ShouldRequestTheApiUserScopes', () => {
    expect(params.scope.split(' ')).toEqual(expect.arrayContaining(['openid', 'profile', 'email', 'read:user', 'write:user']));
  });
});
