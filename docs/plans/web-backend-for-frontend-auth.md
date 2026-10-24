# Web sign-in through the backend (BFF)

Status: **not started** — deferred, recorded 2026-10-24.

## Problem

`holefeeder-web` signs in with the Auth0 SPA SDK and keeps tokens in the
browser: `useRefreshTokens` with `cacheLocation="localstorage"`
(`frontend/apps/holefeeder-web/src/shared/auth/authentication-provider.tsx`).
That keeps a user signed in across page reloads, which the default in-memory
cache cannot do now that browsers block the third-party cookies Auth0's silent
iframe relies on.

The trade-off: any script running on the page can read the access and refresh
tokens from localStorage, so an XSS flaw becomes token theft. Refresh token
rotation in Auth0 limits how long a stolen refresh token stays useful, but the
tokens are still exposed.

## Shape

Move sign-in to the backend (backend-for-frontend pattern):

- The backend runs the Auth0 authorization-code flow as a confidential client
  and holds the tokens server side.
- The browser only gets an `HttpOnly`, `Secure`, `SameSite` session cookie, so no
  token is ever reachable from JavaScript.
- The web app calls the API with that cookie; the backend attaches the access
  token. Anti-CSRF protection is needed once requests ride on a cookie.
- The web app drops `@auth0/auth0-react`; `login`/`logout` become redirects to
  backend endpoints, and the signed-in user comes from an endpoint such as
  `GET api/v2/users/me`.
- Mobile is unaffected — it is a native client and keeps its own Auth0 flow.

## Open questions

- Session storage on the backend (in-memory, PostgreSQL, distributed cache).
- Whether the BFF lives in `Holefeeder.Api` or a separate host behind Traefik.
- How Playwright E2E authenticates once the SPA SDK is gone.
