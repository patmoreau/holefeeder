# Vertical Slice Refactor Plan — holefeeder-mobile

> Goal: each feature slice owns everything it needs and does not cross into another
> feature. Code shared by multiple slices lives in `shared/` (or `@holefeeder/shared/core`)
> only when it is genuine shared kernel — a domain primitive, value object, or config
> used by 3+ slices, or 95%+ identical duplicated code.

Status: **All phases done.** 1 (CategoryType+Settings→shared/core), 2 (accounts↔flows core
cycle broken), 3 (shared form fields + ExpenseTrendBadge + `summary/` read-model slice),
4 (boundary lint enforced). See phase outcomes below.

## Current coupling (audit)

Cross-feature imports found by scanning `@/<feature>/*` imports that cross into a
different `<feature>`. Three distinct kinds:

### 1. Circular dependency: `accounts` ↔ `flows` (the real violation)

```
accounts → flows :  CategoryType, CashflowVariation, FlowsRepository, Transaction,
                    DeleteTransactionUseCase, AmountField, DescriptionField
flows    → accounts:  Account, AccountVariation, AccountForTest
```

Root knot: `FlowsRepository` (flows/core) **returns** `AccountVariation` (accounts/core
type), while `WatchAccountVariationUseCase` (accounts/core) **consumes**
`FlowsRepository`. Mutual dependency at the core layer — the slices cannot be built or
reasoned about independently.

Cause: `AccountVariation` is really a **flows read-model** (transactions aggregated per
account) but is declared in `accounts/core/account-variation.ts`. Wrong home.

### 2. Shared kernel living inside a feature slice

| Type | Declared in | Consumed by |
|---|---|---|
| `Settings` / `DefaultSettings` / `SETTINGS_CODE` | `settings/core/settings.ts` | statistics, dashboard, flows, accounts |
| `CategoryType` / `CategoryTypes` | `flows/core/categories/category-type.ts` | accounts, dashboard |

`Settings` = app-wide reporting-period config. `CategoryType` = value object + multiplier.
Both are shared kernel used by 3–4 slices → belong in `shared/core` (or `@holefeeder/shared/core`),
not inside a feature slice.

### 3. Aggregation slices composing over source slices (`dashboard`, `statistics`)

`dashboard` and `statistics` are read-model / view slices — cross-cutting by nature. They
pull `Transaction`, `UpcomingFlow`, `Account` and use-cases from accounts+flows. This is
**legitimate composition**, but currently reaches into deep internal paths
(e.g. `@/flows/core/flows/pay/pay-use-case`) instead of a slice's public surface.

Presentation leaks in the same category:

- `accounts/EditAccountFormContent` imports `flows/.../AmountField` + `DescriptionField`
- `statistics/InsightsPeriodHeader` imports `dashboard` (`useDashboard`,
  `DashboardHeaderExpenseTrend`, `NO_SUMMARY`)
- `dashboard/AccountCard` imports `accounts/.../use-account-detail`

---

## Refactor plan

Ordered. Each step is a **structural change only** (Tidy First): tests green before and
after, separate commit, no behavioral change mixed in.

### Phase 1 — Extract shared kernel (cheap, mechanical, kills most edges)

- **1a.** Move `CategoryType` → `shared/core/category-type.ts` (value object, no deps).
  Update flows / accounts / dashboard imports.
- **1b.** Move `Settings` type + `DefaultSettings` + `SETTINGS_CODE` →
  `shared/core/settings.ts`. The **settings feature keeps** its persistence / get / save
  use-cases and presentation — only the domain type/default moves to kernel.

Result: statistics→settings edge gone entirely; dashboard/accounts/flows lose their
settings-type edge; accounts/dashboard lose the category-type edge.

### Phase 2 — Break the accounts ↔ flows cycle

- **2a.** Relocate `AccountVariation` → `flows/core/flows/` (it is produced by flows,
  keyed by accountId). `FlowsRepository`'s return type becomes local; the `flows→accounts`
  type edge drops.
- **2b.** Home for `WatchAccountVariationUseCase` — **decide during implementation.** It
  composes `AccountsRepository` + `FlowsRepository` to produce `AccountDetail`, which is
  inherently cross-slice. Candidates:
  - Option A: move into `dashboard` (account-detail is a reporting concern;
    `dashboard` already owns `use-account-details`). accounts/core stops depending on flows.
  - Option B: a dedicated thin `account-detail` composition slice depending on both source
    slices' public API.
  - Inspect `AccountDetail` consumers first, then pick.
  Constraint either way: **accounts/core and flows/core must stop importing each other
  directly.** Only a composition slice may depend on both.
- **2c.** Test doubles: `flows/.../__tests__/cashflow-for-test.ts` imports `AccountForTest`.
  Move test builders alongside their type, or into a shared test-support folder.

### Phase 2 outcome (2b decision)

Done in commit `break accounts<->flows core cycle`:
- `AccountVariation` + its test builder moved `accounts/core` → `flows/core/flows`
  (it is data produced by `FlowsRepository`).
- `cashflow-for-test` dead `AccountForTest` / `CategoryForTest` params + imports removed.

Result: `flows/core` imports zero `accounts/core` — **cycle broken**.

**Residual, one-directional** `accounts/core → flows/core`: a single composition use
case, `WatchAccountVariationUseCase`, which builds `AccountDetail` (account balance +
projected balance) from `AccountsRepository` + `FlowsRepository`. Not a cycle.

**Decision: leave the residual one-directional edge** — accepted as an inherent
composition dependency (account-detail projection legitimately needs flow data). No
extraction. Rationale below.

Fully removing this residual edge is **not cheap**: the account-detail read feature
(type `AccountDetail`, `WatchAccountVariationUseCase`, `use-account-detail` hook,
`AccountScreen`, `AccountHeaderLargeCard`) is consumed *inside* the accounts slice. Any
placement of the use case outside accounts that is still consumed by the accounts screen
re-creates an `accounts ↔ X` cycle, because the use case also needs `accounts/core`
(`Account`, `AccountsRepository`, `AccountType`). The only cycle-free full fix is to
extract the **entire** account-detail vertical into its own composition slice
(`account-detail/`) that depends on `accounts/core` + `flows/core`, leaving `accounts/core`
a pure leaf. That touches presentation and expo-router files → treat as its own phase.

One remaining **test-only** edge: `flows/persistence/flows-repository-in-powersync.spec`
imports `anAccount` to seed accounts in the shared PowerSync DB for an integration test.
Acceptable test arrangement against the shared schema; optionally inline raw SQL later.

### Phase 3 — Presentation

- **3a.** `DescriptionField` is fully generic (deps: shared components + i18n only) →
  move to `shared/presentation/components/fields/`.
- **3b.** `AmountField` couples to `PurchaseType` (flows). Replace that prop with a generic
  `sign`/`variant` prop, then move to `shared/.../fields/`. Removes the accounts→flows
  presentation edge.
- **3c.** `statistics/InsightsPeriodHeader` reuses the dashboard header. Extract the shared
  header (`DashboardHeaderExpenseTrend` + period logic) into `shared/presentation` if truly
  identical; otherwise give statistics its own copy.
- **3d.** `flows/presentation/shared/` is a mini-shared **inside** a slice. The
  account/category/tag field glue (`AccountField`, `CategoryField`, `use-accounts`,
  `use-categories`) is cross-feature → promote the cross-cutting ones to
  `shared/presentation`; keep flow-only ones in flows.

### Phase 3 outcome

Done:
- **3a/3b** — `DescriptionField` + `AmountField` moved to
  `shared/presentation/components/fields`. `AmountField`'s `PurchaseType` prop replaced
  with a generic `tone`; flows maps its type via `flows/presentation/shared/core/amount-tone`.
  Removes the `accounts → flows` presentation-field leak.
- **3c (badge)** — `DashboardHeaderExpenseTrend` extracted to a generic
  `shared/presentation/components/ExpenseTrendBadge` (takes only `{amount,percentage,isOver}`).
  Dashboard cards + statistics header both consume it.
- **3d** — no-op: `flows/presentation/shared/*` (AccountField, CategoryField, use-accounts,
  etc.) is consumed only inside flows; not cross-cutting, stays in flows.

**3c summary edge — resolved (chose relocate).** The current-period spending read-model
was extracted from `dashboard/*` into a neutral **`summary/`** slice (core, persistence,
tests, `use-summary` hook). Renames: `DashboardRepository→SummaryRepository`
(`+InPowersync/InMemory`), `DashboardComputedSummary→ComputedSummary`,
`useDashboard→useSummary`, `dashboardRepository→summaryRepository`. Both the dashboard home
screen and statistics insights header now depend one-way on `summary/`; `statistics` no
longer imports `dashboard`.

### Phase 4 — Enforce boundaries (done)

Implemented with `import-x/no-restricted-paths` in `eslint.config.mjs` (directory-level
zones — the per-slice public `index.ts` barrels from the original plan were judged
unnecessary overhead; directory zones give the same guarantee with no import churn).
Zones (production code only; `*.spec.*` and `__tests__/**` are exempt so integration specs
may seed cross-slice data in the shared PowerSync DB):

- `summary/` may import only `shared/*` — stays a neutral read-model.
- Domain slices (`accounts`, `flows`, `settings`, `user-registration`) must not import a
  view/aggregation slice (`dashboard`, `statistics`, `summary`) — dependencies point the
  other way.
- `statistics` ⇎ `dashboard` — sibling view slices must not couple; share via `summary/` or
  `shared/`.

Not restricted (allowed composition): view/aggregation slices → source slices; the accepted
one-directional `accounts → flows` edge; the composition root in `shared/repositories`
(+ `shared/presentation/core/use-settings`) wiring feature repositories.

Verified: rule flags injected leaf→view, `statistics→dashboard`, and `summary→flows`
imports; current tree passes clean.

---

## Recommended order

Phase 1 first — cheapest, kills the largest edge count, and isolates the real problem (the
accounts↔flows cycle) so Phase 2 can be reasoned about cleanly. Phase 2b is the only step
needing a design decision; make it after inspecting `AccountDetail` consumers.
