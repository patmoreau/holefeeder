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
    └── core/                ← @holefeeder/core — shared platform-agnostic library
```

## Key Commands (run from repo root)

```bash
pnpm install                   # install all workspace dependencies
pnpm build                     # turbo: build all packages in dependency order
pnpm test                      # turbo: test all packages
pnpm lint                      # turbo: lint all packages
pnpm typecheck                 # turbo: typecheck all packages

# Per-workspace
pnpm --filter @holefeeder/core build
pnpm --filter @holefeeder/core test
pnpm --filter holefeeder-mobile test
pnpm --filter holefeeder-mobile ios
```

## `packages/core` — @holefeeder/core

Pure TypeScript library. No React Native, no Expo, no browser-only APIs.

**Exports:**

- **Branded value objects**: `Money`, `Variation`, `Id`, `DateOnly`
- **Result pattern**: `Result<T>`, `AsyncResult<T>` (`Success | Failure | Loading`)
- **Domain helpers**: `DateInterval`, `DateIntervalType`, `withDate`, `buildUrl`, `combineWatchers`
- **Repository interfaces**: `StoreItem`, `StoreItemsRepository`
- **Use-case base types**: `Command`, `Query`
- **Logger**: `Logger` singleton + `loggerFactoryForNoop`
- **Auth types**: `AuthenticationState`, `TokenInfo`, `User`
- **Language type**: `LanguageType`
- **Translations**: `en`, `fr` locale objects + `tk` key-path helper + `TranslationStructure`

**`Id` dependency injection** — `expo-crypto` is NOT imported by `@holefeeder/core`. Each app must call
`Id.setGenerator(fn)` at bootstrap before any call to `Id.newId()`:

```typescript
// apps/holefeeder-mobile/src/app/_layout.tsx
import * as Crypto from 'expo-crypto';
Id.setGenerator(Crypto.randomUUID);
```

**Build:**

```bash
pnpm --filter @holefeeder/core build   # tsc --build (composite project reference)
```

Metro dev mode reads TypeScript directly via the `"source"` field in package.json — no pre-build needed.

## Adding locale strings

Locale strings live in `packages/core/src/translations/locales/`. Add keys to both:

- `packages/core/src/translations/locales/en-CA/translations.ts`
- `packages/core/src/translations/locales/fr-CA/translations.ts`

The mobile app re-exports everything from `@holefeeder/core` via stubs in `src/i18n/locales/*/`.

## Dependency Graph

```text
apps/holefeeder-mobile  ──→  packages/core
apps/holefeeder-web     ──→  packages/core  (planned)
```

Turbo enforces `dependsOn: ["^build"]` so `packages/core` is always built before apps.

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
