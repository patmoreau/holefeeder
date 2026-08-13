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

- ~~A never-registered E2E user~~ — **done** (`f9639d49`). `E2E_NEW_EMAIL` /
  `E2E_NEW_PASSWORD` are in place, `run.sh` mints a second token, and
  `reset-new-user.sh` wipes that user's rows before *each* onboarding flow. All
  four onboarding flows run for real: `pnpm test:e2e:onboarding`.
- ~~`backend/CLAUDE.md` test-command notes~~ — done alongside B3.

## Mobile

Unit tests are mandatory throughout; Maestro covers wiring and navigation.

| Batch | Work | Maestro flow | Status |
|---|---|---|---|
| B6 | `ApiClient.get` + typed HTTP failures — statuses map to kebab-case `ApiErrors` codes instead of `statusText` | — | done (`f45a0a01`, `5f127e94`) |
| B7 | `users-api` + `CheckRegistrationUseCase` / `RegisterUserUseCase` in the existing `user-registration/` module | — | done (`015b5769`) |
| B8 | 3-way gate in `HolefeederContent`: no user → `(auth)`, unregistered → `(onboarding)`, registered → `(app)` | `flows/onboarding/gate.yaml` — passes | done (`87123722`) |
| B9 | Welcome screen replaces Login; `Create account` passes `screen_hint: 'signup'`; i18n en + fr | `flows/auth/signup.yaml` — passes | done (`51b865f3`) |
| B10 | Onboarding: registering screen, progress + retry, waits for first PowerSync sync (capped — see below) | `flows/onboarding/register.yaml` — passes | done (`50710a59`, `3fdfc889`) |
| B11 | Onboarding: budget period, reuses `BudgetSettingsFormContent`, writes `store_items` code=settings | `flows/onboarding/budget-period.yaml` — passes | done (`25e29d47`, `f6d49503`) |
| B12 | Onboarding: first account, reuses the account form, finish → `(app)`. Takes over opening the gate from B11 | `flows/onboarding/first-account.yaml` — passes | done (`24a3ead2`) |
| B13 | Onboarding: suggested categories — localised en/fr suggestions the user accepts or drops, written through PowerSync. Replaces the dropped B5 | `flows/onboarding/categories.yaml` — passes | done (`a5dd3366`) |

### Two traps in this part of the app

**Toolbar buttons cannot be selected by id.** `Stack.Toolbar.Button` accepts no
`testID` — the type system rejects it — and it carries an icon, so neither selector
this repo prefers works. The onboarding steps put their action in a form row instead,
which carries an id. Wrapping `Stack.Toolbar.Button` the way the `App*` components
are wrapped would let the app's own form screens be driven too.

**Maestro cannot type into a field without an id.** Tapping the visible label hits
the label, and the text goes nowhere; the form then refuses to save for a missing
value, which looks like a product bug. Fields the flows drive need a `testID`.
`hideKeyboard` also fails on this setup, and is unnecessary when the save action is
in the header.

**Buttons after a form get clipped.** An `AppButton` placed after the form inside a
SwiftUI column lands past the home indicator, where nothing can tap it. Form actions
belong in the header toolbar here. And `AppScreen` cannot be used in a group whose
header is hidden: its `useHeaderHeight` crashes the app outright.

### Waiting for the first sync is capped

B10 waits for a PowerSync checkpoint that landed *after* registering, but gives up
after ten seconds and goes in anyway. The sync rules resolve the bucket from
`user_identities` through the token subject, so a client that connected before that
row existed may never receive a checkpoint — an uncapped wait hangs on the
onboarding screen forever, which is what the first end-to-end run did. Reconnecting
PowerSync after registering would be the stricter fix if the empty-dashboard moment
ever becomes a problem.

**The feature is complete.** A new user signs up, is registered, sets a budget
period, adds an account, picks categories, and lands in the app — every step
covered by a flow that runs.

Renaming a suggestion in place was dropped from B13: the switches accept or drop
each one, and renaming is what the category screen already does well.

One characteristic of the suite, not the app: a flow that finishes and hands
straight over to the next can lose its last writes, because `clearState` wipes the
local database before PowerSync has uploaded them. The flows assert what is on
screen, so they pass either way — checking the backend afterwards needs a moment's
grace.

The Angular `Holefeeder.Web` and `holefeeder-web` apps are untouched and inherit
the endpoints later.

## Known gap, outside this plan

~~The mobile app is never typechecked in CI~~ — **done** (`7b902d78`). The app
defines `typecheck` now and the eight errors hiding behind the gap are fixed.

One limit worth knowing: `.expo/` is gitignored, so CI typechecks without the
generated route manifest and cannot catch a route path that does not exist. Code
must compile both with and without it — a `@ts-expect-error` that only applies
when the manifest is present becomes an error itself when it is not.
