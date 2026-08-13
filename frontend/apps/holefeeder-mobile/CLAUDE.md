# CLAUDE.md — Holefeeder Mobile

This file provides guidance to Claude Code (claude.ai/code) when working with the mobile app.

> Frontend-wide guidance lives in `../../CLAUDE.md`; project-wide principles live in the
> repo root `../../../CLAUDE.md`.

> [!IMPORTANT]
> **Documentation Maintenance Rules:** Whenever structural or architectural changes are made to the codebase, you **must** update this `CLAUDE.md` file and the frontend `../../CLAUDE.md` to reflect the changes. Keep these instructions accurate so they remain a reliable single source of truth!

## Tech Stack

- React Native (Expo SDK 56)
- expo-router (file-based routing)
- PowerSync + op-sqlite (offline-first sync)
- Auth0 (react-native-auth0)
- i18next
- pnpm
- TypeScript strict
- **@holefeeder/core** (shared domain library)

## Key Commands

Run from **repo root** using `pnpm --filter holefeeder-mobile <cmd>`, or directly from this directory:

```bash
pnpm start                  # dev server (APP_ENV=development)
pnpm ios                    # run on iOS simulator
pnpm android                # run on Android emulator
pnpm test                   # Jest (jest-expo preset)
pnpm test -- --coverage     # with 70% threshold enforcement
pnpm lint                   # expo lint
pnpm prebuild:dev           # iOS prebuild (development)
pnpm ios:deploy             # production build to physical device
```

E2E tests use Maestro, and the two tags need different builds:
`pnpm ios:e2e && pnpm test:e2e:ios` (tag `regression`, session injected by link) and
`pnpm ios:e2e:auth && pnpm test:e2e:auth` (tag `auth`, real Auth0 pages). Both need a
booted simulator, the local Docker stack, and credentials in `.maestro/.env.e2e.local`
(gitignored — copy `.maestro/.env.e2e.template`). They live under `.maestro/` and not
in the app root because Metro treats any `.env*.local` there as a source file and
fails to bundle in dev. See `.maestro/README.md`.

## Architecture: Feature-Based Vertical Slices

The app relies on feature-based modules to isolate domain logic and UI alongside shared cross-cutting layers:

```text
src/[feature]/            ← Feature modules (e.g. accounts/, flows/, settings/, statistics/, dashboard/, summary/, user-registration/)
  core/         ← Entities, value objects, repository interfaces, use cases (not all features have this)
  persistence/  ← PowerSync implementations of repository interfaces (not all features have this)
  presentation/ ← Feature-specific UI and presentation hooks
src/shared/               ← Cross-cutting shared modules
  api/          ← API service layer
  auth/         ← Auth state and logic
  core/         ← Shared domain primitives + kernel value objects shared by 3+ slices
                  (Money, Id, Result; plus CategoryType, Settings, inactive, system)
  hooks/        ← Cross-cutting React hooks
  language/     ← Language selection state and provider
  persistence/  ← App-wide PowerSync db schema and utilities
  presentation/ ← Shared global UI components
  repositories/ ← Shared repository providers and contexts
  theme/        ← Design system styling and tokens
src/app/                  ← expo-router route files only
src/config/config.ts      ← All env-var config (never read process.env directly elsewhere)
src/i18n/                 ← i18next, en-CA and fr-CA locales
src/types/icons.ts        ← AppIcons (SF Symbols) + AppIconsMapping (→ Material Icons)
```

**Slice boundaries.** A feature slice must not import another feature slice. Code shared by
3+ slices (or a genuine domain value object / read-model) moves to a neutral home:
`shared/*` for kernel + generic UI (e.g. `shared/presentation/components/fields` for generic
form inputs like `AmountField`, `DescriptionField`), or its own read-model slice. `summary/`
is one such read-model slice (current-period spending) consumed one-way by both the
`dashboard` and `statistics` view slices. Composition/read slices (`dashboard`, `statistics`,
`summary`) may depend on source slices' public API; a single documented one-directional
`accounts → flows` edge remains for account-detail projection. See
`docs/vertical-slice-refactor.md`.

## Shared Domain Library (`@holefeeder/core`)

Branded value objects, Result pattern, logger, auth types, and locale strings live in `packages/core`
and are published as `@holefeeder/core`. Import from `@holefeeder/core` directly, or via the re-export
stubs in `src/shared/core/` and `src/i18n/` which preserve the old `@/shared/core/...` import paths.

At app bootstrap (`src/app/_layout.tsx`) the UUID generator and logger factory must be injected:

```typescript
import * as Crypto from 'expo-crypto';
import { Id } from '@holefeeder/core';
Id.setGenerator(Crypto.randomUUID);
```

## Branded Value Objects

Domain primitives use nominal typing via brand: `Money`, `Variation`, `Id`, `DateOnly`.

- **`create(value)`** — validates and returns `Result<T>` (use when input is untrusted)
- **`valid(value)`** — bypasses validation (use only for already-trusted DB rows)
- Monetary values are stored as **integer cents** in PowerSync; convert with `Variation.fromCents()` /
  `Money.fromCents()`

## Result / AsyncResult Pattern

All domain operations return `Result<T>` (`Success<T> | Failure`) or `AsyncResult<T>` (adds `Loading`).

```typescript
if (result.isSuccess) result.value; // T
if (result.isFailure) result.errors; // string[]
if (result.isLoading)
  /* show spinner */

  Result.combine({ a: resultA, b: resultB }); // merges multiple results
Result.combineArray(resultArray);
```

## Repository + Use-Case Pattern

1. **Interface** in `src/<feature>/core/<entity>/<entity>-repository.ts`
2. **Implementation** in `src/<feature>/persistence/<entity>/<entity>-repository-in-powersync.ts` — uses PowerSync
   `.query().watch()` listener pattern
3. **Use case** in `src/<feature>/core/<entity>/<action>/` — either `Command` (`.execute()`) or `Query` (`.query(onChange)`
   returns unsubscribe fn)
4. **Presentation hook** in `src/<feature>/presentation/hooks/` — calls `useRepositories()`, manages
   `useState<AsyncResult<T>>` + `useEffect` cleanup

Example flow: `useAccounts` → `WatchAccountsUseCase` → `AccountsRepository` → `AccountsRepositoryInPowersync`

## Reactive Query Hook Pattern

```typescript
const useAccounts = (): AsyncResult<Account[]> => {
  const { accountRepository } = useRepositories();
  const [accounts, setAccounts] = useState<AsyncResult<Account[]>>(
    Result.loading(),
  );
  const useCase = useMemo(
    () => WatchAccountsUseCase(accountRepository),
    [accountRepository],
  );
  useEffect(() => {
    const unsubscribe = useCase.query(setAccounts);
    return () => unsubscribe();
  }, [useCase]);
  return accounts;
};
```

For multiple concurrent watches, use `useMultipleWatches` from `src/shared/presentation/core/use-multiple-watches.ts`.

## Form Pattern

Use `createFormDataContext<FormData, ErrorEnum>(displayName, saveFn)` from
`src/shared/presentation/core/use-form-context.tsx` to get a typed context factory with built-in: dirty tracking,
field-level + general errors, validation, save, and an `ErrorSheet` component.

## Provider Nesting (root `_layout.tsx`)

`LanguageProvider` → `ThemeProvider` → `AuthenticationProvider` → `PowerSyncAuthProvider` → `RepositoryProvider`  
(the auth layer becomes `E2eAuthenticationProvider` in E2E builds — see Config / Environments)  
Route protection uses `<Stack.Protected guard={!!user}>` inside `HolefeederContent` — no manual redirects.

## Config / Environments

- `APP_ENV=development|production` selects `.env.development` / `.env.production`
- All config read through `src/config/config.ts`; never access `process.env` directly in components
- Local services: `powersync.localtest.me` and `holefeeder.localtest.me` via Traefik + mkcert TLS  
  (iOS Simulator requires `/etc/hosts` override and `xcrun simctl keychain <UDID> add-root-cert`)
- `pnpm ios:e2e` builds the **E2E variant**: development endpoints plus `EXPO_PUBLIC_E2E=true`.
  There is no `.env.e2e` — the flag is passed on the command line, and dotenv does not
  override variables already set in the environment.

### E2E authentication

When `EXPO_PUBLIC_E2E=true`, `_layout.tsx` mounts `E2eAuthenticationProvider` instead of
`AuthenticationProvider`. It never opens a browser: it waits for a
`holefeeder://?e2e-auth-token=<jwt>` deep link and serves that token through the same
`AuthenticationContext`, so everything downstream (`useAuth`, `PowerSyncAuthProvider`,
`ApiClient`) is unchanged. Subject and expiry are read from the token, not from the link.

The link targets the **root** path on purpose. expo-router turns any other host into a
route, and an unknown route fails the render with "Element type is invalid".

This is an authentication bypass and must never ship. Two guards, both required:

1. `app.config.ts` sets `EXPO_PUBLIC_E2E` to `undefined` whenever `APP_ENV=production`, so a
   production build cannot carry the flag regardless of what any env file contains.
2. `E2eConfig.isEnabled()` accepts only the exact string `'true'`.

## Path Aliases

`@/` → `src/` (primary alias for all app code)  
`@tests/` → `tests/` (test utilities only)

## Icons

Always use `AppIcons.<key>` (SF Symbol name) rather than raw strings. Add new icons to both `AppIcons` and
`AppIconsMapping` in `src/types/icons.ts`.

## Native UI Wrappers (`@expo/ui` boundary)

`@expo/ui` must **never** be imported outside `src/shared/presentation/components/native/`
(the `modules/` SwiftUI glue is the only other exception). An ESLint `no-restricted-imports`
rule fails the build on any violation. Feature code always consumes the `App*` wrappers, never
`@expo/ui` directly.

Two-layer wrapper pattern inside `native/`:

- `expo/Expo<Name>.tsx` — thin passthrough over the raw `@expo/ui` component
  (`@expo/ui/swift-ui` for iOS-native primitives).
- `App<Name>.tsx` — the app-facing component built on its `Expo<Name>`, adding theme-aware
  props and defaults. Feature code imports this.

When feature code needs an `@expo/ui` component, hook, or modifier that has no wrapper yet,
**add one** rather than importing `@expo/ui` directly:

- **Component** → new `Expo<Name>` + `App<Name>` pair (e.g. `AppDivider`, `AppSection`,
  `AppProgressView`).
- **Modifier** → expose it on `AppModifiers` (e.g. `AppModifiers.frame`, `.onAppear`), or add
  a prop to the relevant `App*` component (e.g. `AppList`'s `inset` wraps `listStyle('inset')`).
- **Icon name for a native prop** (toolbar/tab `icon=`) → `AppIcon.select(AppIconMap.x)`.
- **Hook** → re-export from `native/` (e.g. `use-native-state.ts` re-exports `useNativeState`).

Types that would couple a non-`native/` module to `@expo/ui` are redefined locally when
structurally safe (e.g. `ButtonRole` in `AppButtonVariant.ts`).

**`testID` forwarding.** Every `App*` wrapper must accept `testID` and pass it to its native
view — `@expo/ui` maps it to `accessibilityIdentifier`, which is how Maestro selects elements.
Wrappers that spread props (`{...props}`) get this for free; wrappers that enumerate their
props must declare `testID?: string` and forward it on every return branch. Composite wrappers
(e.g. `AppField`) put it on their outermost native view. Naming convention:
`<screen>-<element>` in kebab-case, e.g. `welcome-signup-button`.

## i18n

All user-facing strings must use `useTranslation()` from react-i18next. Add keys to both locale files
in **`packages/core/src/translations/locales/`** (en-CA and fr-CA) — the files in `src/i18n/locales/`
are thin re-export stubs that point to `@holefeeder/core`.

## Database Schema (PowerSync)

The app syncs offline-first via PowerSync (SQLite). All tables use a `user_id` column to isolate multi-tenant data. Monetary values (`amount`, `open_balance`, `budget_amount`) are stored strictly as **positive integer cents**. In other words, all amounts and balances are strictly positive in the database. When calculating a balance: if the associated category type or account type is an expense/liability, a multiplier of `-1` is applied. If the category or account type is a gain/asset, a multiplier of `1` is applied. Convert via `Money.fromCents()` or `Variation.fromCents()`.

- **`accounts`**: User financial accounts. Includes `type`, `name`, `favorite`, `open_balance`, `open_date`, `description`, `inactive`, `user_id`.
- **`categories`**: Transaction categories. Includes `type`, `name`, `color`, `budget_amount`, `favorite`, `system`, `inactive`, `user_id`. Soft deleted via the `inactive` flag (never hard deleted, so existing transactions keep resolving).
- **`cashflows`**: Recurring transactions/bills. Includes `effective_date`, `amount`, `interval_type`, `frequency`, `recurrence`, `description`, `account_id`, `category_id`, `inactive`, `tags`, `user_id`.
- **`transactions`**: Individual ledger transactions. Includes `date`, `amount`, `description`, `account_id`, `category_id`, `cashflow_id`, `cashflow_date`, `tags`, `user_id`.
- **`store_items`**: Key-value pairs for arbitrary user data. Includes `code`, `data`, `user_id`.

## Testing Strategy

This application enforces a testing strategy focused on high domain reliability and E2E core flows.

- **Unit & Integration Tests (Jest)**:
  - Uses `jest` with `jest-expo` preset. Run tests using `pnpm test`.
  - Test files are named `*.spec.ts(x)` or `*.test.ts(x)`. Files within `__tests__/` are counted towards coverage.
  - Do not test react-native UI components as they are untestable. Hooks must be tested.
  - Focus on **fakes** instead of mocks for state and persistence. For example, use in-memory repository fakes (`repositories-in-memory`) instead of creating real database instances or heavily mocking methods. Global mocks/fakes are located in `__mocks__/` at root, context helpers in `tests/setup/`.
  - **Coverage Threshold**: Strictly enforced at 70% for branches, functions, lines, and statements (`pnpm test -- --coverage`).
- **E2E Tests (Maestro)**:
  - User flows are verified using Maestro. Flows live in `.maestro/flows/`, reusable fragments in `.maestro/subflows/`.
  - Every flow must carry a tag or it never runs: `regression` for fast deterministic flows (`pnpm test:e2e:ios`, needs the `ios:e2e` build), `auth` for flows that drive the real Auth0 hosted pages (`pnpm test:e2e:auth`, needs the `ios:e2e:auth` build), `onboarding` for the new-user journey (`pnpm test:e2e:onboarding`, needs the `ios:e2e` build and the local stack). `run.sh` refuses to run a tag against the wrong build.
  - `onboarding` flows use a second Auth0 user (`E2E_NEW_EMAIL`) and are run one at a time, each preceded by `reset-new-user.sh`, which wipes that user's backend rows so the journey starts from nothing every time.
  - Select elements by `id:` (the `testID` prop), not by visible text — text is translated. Text selectors are only unavoidable inside the Auth0 pages.
  - Credentials load from `.maestro/.env.e2e.local` via `.maestro/run.sh`; see `.maestro/README.md`.
