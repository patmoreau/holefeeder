# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> Project-wide principles — TDD cycle, Tidy First, commit discipline, and the "Never" list —
> live in the root `../CLAUDE.md`. This file is the single source of truth for the **frontend**,
> shared by all AI agents (Claude, Cursor, Copilot, Gemini). Per-app details live in
> `apps/holefeeder-mobile/CLAUDE.md`.

> [!IMPORTANT]
> **Documentation Maintenance Rules:** Whenever structural or architectural changes are made to the codebase, complete the following steps:
>
> 1. Update this frontend `CLAUDE.md` to reflect the changes.
> 2. Update the relevant per-app `CLAUDE.md` files to reflect the changes.
> 3. Ensure these instructions are updated to reflect all structural or architectural changes in the codebase, with specific examples where applicable.

## Workflow — Test-Driven Development

This project is developed with **TDD**. For **every new feature**, BEFORE writing any
implementation code:

1. **State the test plan first.** Enumerate the cases you will cover — happy path, edge
   cases, exclusions, aggregation/averaging, and error handling — and let the author
   confirm or adjust before implementation starts.
2. **Then implement**, writing the tests first.

Match the depth of
`apps/holefeeder-mobile/src/statistics/persistence/insights-repository-in-powersync.spec.ts`,
the reference for a good test in this repo:

- Runs against a real database (`setupDatabaseForTest`), not mocks.
- Uses builders (`aCategory`, `aTransaction`, `aSettings`) to arrange realistic, varied
  data across multiple periods.
- Covers the happy path **and** edge cases: zero/empty values, exclusions (gain
  categories, system categories), averaging across previous periods, and database-error
  handling.
- Asserts precise expected values (`toBeSuccessWithValue`, `toBeFailureWithErrors`), not
  just "is defined".

## Monorepo Structure

This is a **pnpm + Turborepo** monorepo.

```text
holefeeder-ui/
├── apps/
│   ├── holefeeder-mobile/   ← Expo / React Native app  →  CLAUDE.md inside
│   └── holefeeder-web/      ← React 19 + Vite + MUI web app (planned)
└── packages/
    └── shared/              ← @holefeeder/shared — shared platform-agnostic library
```

## Key Commands (run from repo root)

```bash
pnpm install                   # install all workspace dependencies
pnpm build                     # turbo: build all packages in dependency order
pnpm test                      # turbo: test all packages
pnpm lint                      # turbo: lint all packages
pnpm typecheck                 # turbo: typecheck all packages

# Per-workspace
pnpm --filter @holefeeder/shared build
pnpm --filter @holefeeder/shared test
pnpm --filter holefeeder-mobile test
pnpm --filter holefeeder-mobile ios
```

End-to-end tests (Maestro) live in `apps/holefeeder-mobile/.maestro/` and need a booted
simulator, so they are not part of `pnpm test` or CI. See that app's `CLAUDE.md`.

## `packages/shared` — @holefeeder/shared

Pure TypeScript library. No React Native, no Expo, no browser-only APIs.

There is **no root export** — the package exposes exactly two subpaths, so always import from
one of them:

- `@holefeeder/shared/core` — everything listed below
- `@holefeeder/shared/testkit` — test-only helpers and builders

**`/core` exports:**

- **Branded value objects**: `Money`, `Variation`, `Id`, `DateOnly`
- **Result pattern**: `Result<T>`, `AsyncResult<T>` (`Success | Failure | Loading`)
- **Domain helpers**: `DateInterval`, `DateIntervalType`, `withDate`, `buildUrl`, `combineWatchers`
- **Repository interfaces**: `StoreItem`, `StoreItemsRepository`
- **Use-case base types**: `Command`, `Query`
- **Logger**: `Logger` singleton + `loggerFactoryForNoop`
- **Auth types**: `AuthenticationState`, `TokenInfo`, `User`
- **Language type**: `LanguageType`
- **Translations**: `en`, `fr` locale objects + `tk` key-path helper + `TranslationStructure`

**`Id` dependency injection** — `expo-crypto` is NOT imported by `@holefeeder/shared`. Each app must call
`Id.setGenerator(fn)` at bootstrap before any call to `Id.newId()`:

```typescript
// apps/holefeeder-mobile/src/app/_layout.tsx
import * as Crypto from 'expo-crypto';
import { Id } from '@holefeeder/shared/core';
Id.setGenerator(Crypto.randomUUID);
```

**Build:**

```bash
pnpm --filter @holefeeder/shared build   # tsc --build (composite project reference)
```

Both subpath exports resolve to `dist/`, so the package has to be built before apps resolve
it. Turbo's `dependsOn: ["^build"]` handles that for `build`/`test`/`lint`/`typecheck`. The
mobile Metro config adds `packages/shared` to `watchFolders` for hot reload during dev.

## Adding locale strings

Locale strings live in `packages/shared/src/core/translations/locales/`. Add keys to both:

- `packages/shared/src/core/translations/locales/en-CA/translations.ts`
- `packages/shared/src/core/translations/locales/fr-CA/translations.ts`

The mobile app re-exports everything from `@holefeeder/shared/core` via stubs in `src/i18n/locales/*/`.

## Dependency Graph

```text
apps/holefeeder-mobile  ──→  packages/shared
apps/holefeeder-web     ──→  packages/shared  (planned)
```

Turbo enforces `dependsOn: ["^build"]` so `packages/shared` is always built before apps.

## Git

- Do not commit

## Code

### Typescript

- Use type for everything by default. Only use interface when you specifically need one of its unique features.

### Naming files

- PascalCase for React components. ex: `AppButton.tsx`
- kebab-case for any other file. ex: `date-interval.ts`
- PascalCasefor functions that contains constructors (curried functions). ex: `WatchAccountsUseCase(accountsRepository: AccountsRepository)`

### Test extensions

- Use .spec.ts{x}.

### Enums

- Plural for enum name
- Define using as const and typeof pattern

```typescript
export const DateIntervalTypes = {
  daily: 'daily',
  weekly: 'weekly',
  monthly: 'monthly',
  yearly: 'yearly',
  oneTime: 'oneTime',
} as const;

export type DateIntervalType = (typeof DateIntervalTypes)[keyof typeof DateIntervalTypes];
```

### Errors

- Create an enum to define errors
- Name error using pattern {error}{fieldName}
- Use kebab-case for value

```typescript
export const AccountErrors = {
  invalidName: 'invalid-name',
  requiredOpenBalance: 'required-open-balance',
};
```

### Exporting constant

- Export named constants: This allows better navigation with the IDE. The left side provides access to usage and the right side to implementation. It's also easier for the IDE to rename one of these constants.

```typescript
✅ export const WatchAccountsUseCase = {
  watch: watch,
};
```
instead of:

```typescript
❌ export const WatchAccountsUseCase = {
  watch,
};
```
