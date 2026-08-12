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
| B3 | `POST api/v2/users/register` — 200 / 400 already-exists, policy `WriteUser`, user + identity only | `d431839b`, `c10784de` | done |
| B4 | register seeds system categories Transfer In / Transfer Out (`docs/business-rules/category.md`) | `ba68c634`, `11078afc`, `e04d8b5a`, `25591cd9` | done |
| ~~B5~~ | ~~register seeds starter expense categories~~ — **dropped**, moved to onboarding as B13 | | dropped |

Existing production users are safe: `/me` answers 200 for them, register answers
400, no migration needed.

**The backend is complete.** Registration mints the user, its identity, and the
two system transfer categories.

### Why starter categories are not seeded

B5 was built and reverted. Any name the backend writes is frozen text in a
database row: a French user seeded with "Groceries" keeps it forever, and
changing the app language later cannot fix it. Seeding from `Accept-Language`
only moves the freeze to signup time.

The client knows the locale, so the suggestions belong there — the same
reasoning that already keeps budget period and first account out of
registration. See B13.

The transfer pair stays in the backend because `Transfer` resolves those two
**by name**: they are identifiers, not labels. They are also visible in the
category list, so a French user sees English there. That is a pre-existing wart,
not something B5 introduced, and deserves its own batch.

### Still open

- **E2E reset script**, deferred from B0.4 and again from B3. Wipes the
  `e2e-new@…` user's backend rows so onboarding flows are repeatable.
  `/users/register` now exists, so the only thing missing is that user's
  credentials in `.env.e2e.local`.
- ~~`backend/CLAUDE.md` test-command notes~~ — done alongside B3.

## Mobile

Unit tests are mandatory throughout; Maestro covers wiring and navigation.

| Batch | Work | Maestro flow | Status |
|---|---|---|---|
| B6 | `ApiClient.get` + typed HTTP failures — statuses map to kebab-case `ApiErrors` codes instead of `statusText` | — | done (`f45a0a01`, `5f127e94`) |
| B7 | `users-api` + `CheckRegistrationUseCase` / `RegisterUserUseCase` in the existing `user-registration/` module | — | done (`015b5769`) |
| B8 | 3-way gate in `HolefeederContent`: no user → `(auth)`, unregistered → `(onboarding)`, registered → `(app)` | `flows/onboarding/gate.yaml` | next |
| B9 | Welcome screen replaces Login; `Create account` passes `screen_hint: 'signup'`; i18n en + fr | `flows/auth/signup.yaml` | |
| B10 | Onboarding: registering screen, progress + retry, waits for first PowerSync sync | `flows/onboarding/register.yaml` | |
| B11 | Onboarding: budget period, reuses `BudgetSettingsForm`, writes `store_items` code=settings, removes the silent `DefaultSettings` fallback | `flows/onboarding/budget-period.yaml` | |
| B12 | Onboarding: first account, reuses the account form, finish → `(app)` | `flows/onboarding/first-account.yaml` | |
| B13 | Onboarding: suggested categories — localised en/fr suggestions the user accepts, renames, or skips, written through PowerSync. Replaces the dropped B5 | `flows/onboarding/categories.yaml` | |

B13 can sit anywhere in the onboarding sequence; after the first account is the
natural spot, since categories are only useful once there is somewhere to spend
from. Skipping it must leave a usable app — a user with no expense category can
still create one from the normal category screen.

The Angular `Holefeeder.Web` and `holefeeder-web` apps are untouched and inherit
the endpoints later.

## Known gap, outside this plan

CI runs `pnpm turbo run typecheck`, but only `packages/shared` defines a
`typecheck` script — the mobile app is never typechecked. Eight `tsc` errors in
test files and helpers survive because of it. Adding `"typecheck": "tsc
--noEmit"` to the mobile package and fixing them deserves its own batch.
