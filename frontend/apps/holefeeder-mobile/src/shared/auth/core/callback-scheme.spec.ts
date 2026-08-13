import { callbackSchemeOf } from '@/shared/auth/core/callback-scheme';

describe('callbackSchemeOf', () => {
  it('takes the scheme from a configured Auth0 callback uri', () => {
    const uri = 'com.drifterapps.holefeeder-react.auth0://dev-tenant.ca.auth0.com/ios/com.drifterapps.holefeeder-react/callback';

    expect(callbackSchemeOf(uri)).toBe('com.drifterapps.holefeeder-react.auth0');
  });

  it('handles an ordinary https uri', () => {
    expect(callbackSchemeOf('https://holefeeder.example.app/logout')).toBe('https');
  });

  it('returns nothing when there is no scheme to take', () => {
    expect(callbackSchemeOf('holefeeder-react/callback')).toBeUndefined();
    expect(callbackSchemeOf('')).toBeUndefined();
    expect(callbackSchemeOf('://missing')).toBeUndefined();
  });
});
