# Holefeeder UI — Agent Guide

> [!IMPORTANT]
> **Documentation Maintenance Rules:** Whenever structural or architectural changes are made to the codebase, you **must** update this `AGENTS.md` file and any per-app `AGENTS.md` to reflect the changes. Keep these instructions accurate so they remain a reliable single source of truth!

## Monorepo Structure

This is a **pnpm + Turborepo** monorepo.

```text
holefeeder-ui/
├── apps/
│   ├── holefeeder-mobile/   ← Expo / React Native app  →  AGENTS.md inside
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
pnpm --filter holefeeder-react test
pnpm --filter holefeeder-react ios
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
