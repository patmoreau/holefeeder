# User registration and onboarding

Status log for the feature. Update the status column as batches land.

## Problem

Auth0 authenticates anyone, but nothing ever created the `users` and
`user_identities` rows the rest of the system keys off. `UserContext` resolves
the caller through `user_identities`, and the PowerSync bucket does the same, so
a first-time caller gets `UserId.Empty` and syncs nothing. The mobile app has no
welcome or signup path either — a new user lands in a half-configured app.

## Shape

One commit per batch, TDD, backend before mobile so the app calls real
endpoints. Budget period and first account are **not** seeded at registration —
the onboarding screens write them through the normal PowerSync flow, which
avoids two code paths for the same data.

Contract, as agreed:

- `POST api/v2/users/register` — 200 on create, 400 when the identity already exists
- `GET api/v2/users/me` — 200 with the user id when registered, 404 when not

## B0 — Maestro foundation

| Batch | Work | Commit | Status |
|---|---|---|---|
| B0.1 | `testID` passthrough on `App*` wrappers, `<screen>-<element>` convention | `9dd46bd9` | done |
| B0.2 | `.maestro/` skeleton + real-Auth0 login subflow | `4ba80248`, `24b9636c` | done |
| B0.3 | E2E build variant + `holefeeder://e2e-auth` token injection | `13725892`, `087ea295` | done |
| B0.4 | `login-injected` subflow + token minting in `run.sh` | `53490f22` | done |
| B0.5 | CI `assert:production-safe` guard | `d0c2a91c` | done |

**Security constraint carried through B0.3–B0.5.** The deep-link handler is an
authentication bypass. It is dead code in any non-E2E build: module registration
is gated on `process.env.EXPO_PUBLIC_E2E === 'true'` at import time so the
bundler strips it, the flag is absent from `.env.production`, and CI fails if a
production config carries it. Password-grant credentials live in a gitignored
`.env.e2e.local`; the grant type stays enabled only on the dev tenant.

## Backend

| Batch | Work | Commit | Status |
|---|---|---|---|
| B1 | domain `User.Register(identityObjectId)` — user + identity minted together | `284e851e` | done |
| B2 | `GET api/v2/users/me` — 200 / 404 / 401 | `8fcb6f4c` | done |
| B3 | `POST api/v2/users/register` — 200 / 400 already-exists, policy `WriteUser`, user + identity only | | next |
| B4 | register seeds system categories Transfer In / Transfer Out (`docs/business-rules/category.md`) | | |
| B5 | register seeds starter expense categories, non-system, `favorite=false` — list needed from Patrick | | |

Existing production users are safe: `/me` answers 200 for them, register answers
400, no migration needed.

### Carried into B3

- **E2E reset script**, deferred from B0.4. Wipes the `e2e-new@…` user's backend
  rows so onboarding flows are repeatable. Needs that user's credentials in
  `.env.e2e.local` and needs `/users/register` to exist before there is anything
  meaningful to delete.
- **`backend/CLAUDE.md` notes**: functional tests need
  `TESTCONTAINERS_RYUK_DISABLED=true` under the local podman setup (Ryuk fails
  with `making volume mountpoint … operation not supported`), plus the
  `--filter-class` correction.

## Mobile

Unit tests are mandatory throughout; Maestro covers wiring and navigation.

| Batch | Work | Maestro flow | Status |
|---|---|---|---|
| B6 | `ApiClient.get` + HTTP status on failure (structural) — today every non-ok collapses to `Result.failure([statusText])`, so 404 is indistinguishable from 500 | — | |
| B7 | `users-api` + `CheckRegistrationUseCase` / `RegisterUserUseCase` in the existing `user-registration/` module | — | |
| B8 | 3-way gate in `HolefeederContent`: no user → `(auth)`, unregistered → `(onboarding)`, registered → `(app)` | `flows/onboarding/gate.yaml` | |
| B9 | Welcome screen replaces Login; `Create account` passes `screen_hint: 'signup'`; i18n en + fr | `flows/auth/signup.yaml` | |
| B10 | Onboarding: registering screen, progress + retry, waits for first PowerSync sync | `flows/onboarding/register.yaml` | |
| B11 | Onboarding: budget period, reuses `BudgetSettingsForm`, writes `store_items` code=settings, removes the silent `DefaultSettings` fallback | `flows/onboarding/budget-period.yaml` | |
| B12 | Onboarding: first account, reuses the account form, finish → `(app)` | `flows/onboarding/first-account.yaml` | |

The Angular `Holefeeder.Web` and `holefeeder-web` apps are untouched and inherit
the endpoints later.

## Known gap, outside this plan

CI runs `pnpm turbo run typecheck`, but only `packages/shared` defines a
`typecheck` script — the mobile app is never typechecked. Eight `tsc` errors in
test files and helpers survive because of it. Adding `"typecheck": "tsc
--noEmit"` to the mobile package and fixing them deserves its own batch.
