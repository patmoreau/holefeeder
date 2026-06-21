# Insights Tab — Implementation Progress

Goal: replace the empty Accounts tab with an Insights tab that shows spending by category and tag for the current budget period.

The feature lives in `apps/holefeeder-mobile/src/statistics/`, following the same 4-layer pattern as `dashboard/`:
```
statistics/
  core/               ← domain types + repository contract + use-cases
  persistence/        ← PowerSync SQL
  presentation/core/  ← hooks
  presentation/       ← React components
```

---

## Increment 1 — Rename the Tab ✅

**Goal:** "Insights" appears in the tab bar with a bar-chart icon.

| File | Change |
|------|--------|
| `packages/shared/src/core/translations/locales/en-CA/translations.ts` | Add `tabs.insights` + `insights.title` |
| `packages/shared/src/core/translations/locales/fr-CA/translations.ts` | Mirror French |
| `src/shared/presentation/core/app-icon-map.ts` | Add `insights: { ios: 'chart.bar.fill', android: Analytics }` |
| `src/app/(app)/(tabs)/_layout.tsx` | Trigger name `"statistics"`, insights icon + label |
| `src/app/(app)/(tabs)/statistics.tsx` *(new)* | Route file re-exporting `InsightsScreen` |
| `src/statistics/presentation/InsightsScreen.tsx` *(new)* | Placeholder screen with `ScreenTitle` |
| `src/app/(app)/(tabs)/accounts.tsx` *(deleted)* | Replaced by `statistics.tsx` |
| `src/accounts/presentation/accounts.tsx` *(deleted)* | No longer referenced |
| `src/dashboard/presentation/locals/` *(deleted)* | Dead feature-scoped i18n namespace removed |

---

## Increment 2 — Spending by Category ✅

**Goal:** Every expense category shows its spent amount and over-budget highlighting — live-updating via PowerSync.

| File | What it does |
|------|--------------|
| `src/statistics/core/category-spending.ts` | Domain type: `{ categoryId, categoryName, color, budgetAmount, spentAmount }` |
| `src/statistics/core/insights-repository.ts` | Contract: `watchCategorySpending(onDataChange, settings) => () => void` |
| `src/statistics/core/watch-category-spending/watch-category-spending-use-case.ts` | Thin delegation to repository |
| `src/statistics/core/watch-category-spending/watch-category-spending-use-case.spec.ts` | Unit test ✱ |
| `src/statistics/persistence/insights-repository-in-powersync.ts` | SQL + `watchQuery` |
| `src/statistics/persistence/insights-repository-in-powersync.spec.ts` | Unit test ✱ |
| `src/statistics/presentation/core/use-category-spending.ts` | Hook — mirrors `use-dashboard.ts` pattern |
| `src/statistics/presentation/CategorySpendingCard.tsx` | Name + amount + over-budget label using `AppListItem` |
| `src/statistics/presentation/CategorySpendingList.tsx` | `AppFieldSection` wrapping the cards |
| `src/shared/repositories/core/repositories-state.ts` | Add `insightsRepository: InsightsRepository` |
| `src/shared/repositories/presentation/RepositoryContext.tsx` | Wire `InsightsRepositoryInPowersync(database)` |
| Both locale files | Add `insights.categoryBreakdown.{ title, empty, overBudget, noBudget }` |

> ✱ Unit tests cover `core/` and `persistence/` only. `presentation/` files are not tested.

### Key SQL

```sql
SELECT c.id AS categoryId, c.name AS categoryName, c.color,
       c.budget_amount AS budgetAmount,
       COALESCE(SUM(t.amount), 0) AS spentAmount
FROM categories c
LEFT JOIN transactions t
  ON t.category_id = c.id AND t.date >= ? AND t.date <= ?
WHERE c.system = 0 AND lower(c.type) = 'expense'
GROUP BY c.id ORDER BY spentAmount DESC
```

---

## Increment 3 — Spending by Tag ✅

**Goal:** A second section lists every tag used in the current period with its total spent amount, live-updating.

| File | What it does |
|------|--------------|
| `src/statistics/core/tag-spending.ts` | Domain type: `{ tag, spentAmount }` |
| `src/statistics/core/watch-tag-spending/watch-tag-spending-use-case.ts` | Thin delegation |
| `src/statistics/core/watch-tag-spending/watch-tag-spending-use-case.spec.ts` | Unit test ✱ |
| `src/statistics/presentation/core/use-tag-spending.ts` | Hook — same pattern as `use-category-spending.ts` |
| `src/statistics/presentation/TagSpendingCard.tsx` | Tag icon + name + amount using `AppListItem` |
| `src/statistics/presentation/TagSpendingList.tsx` | `AppFieldSection` wrapping the cards |
| `src/statistics/core/insights-repository.ts` | Add `watchTagSpending` |
| `src/statistics/persistence/insights-repository-in-powersync.ts` | Add recursive CTE SQL method |
| `src/statistics/persistence/insights-repository-in-powersync.spec.ts` | Extend with tag spending tests ✱ |
| Both locale files | Add `insights.tagBreakdown.{ title, empty }` |

> ✱ Unit tests cover `core/` and `persistence/` only. `presentation/` files are not tested.

### Key SQL

```sql
WITH RECURSIVE split(tag, remainder, amount) AS (
  SELECT
    Ltrim(Substr(t.tags || ',', 1, Instr(t.tags || ',', ',') - 1)),
    Substr(t.tags || ',', Instr(t.tags || ',', ',') + 1),
    t.amount
  FROM transactions t
  JOIN categories c ON c.id = t.category_id
  WHERE t.tags IS NOT NULL AND t.tags <> ''
    AND t.date >= ? AND t.date <= ?
    AND lower(c.type) = 'expense' AND c.system = 0
  UNION ALL
  SELECT
    Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)),
    Substr(remainder, Instr(remainder, ',') + 1),
    amount
  FROM split WHERE remainder <> ''
)
SELECT tag, SUM(amount) AS spentAmount
FROM split WHERE tag <> ''
GROUP BY tag ORDER BY spentAmount DESC
```

---

## Increment 4 — Period Header & Polish ✅

**Goal:** Screen shows the current period date range as a subtitle.

| File | What it does |
|------|--------------|
| `src/statistics/presentation/InsightsPeriodHeader.tsx` | Reads `useSettings()`, calls `DateInterval.createFrom(today(), 0, ...)`, renders formatted date range (e.g. "Jun 1 – Jun 30") |
| `src/statistics/presentation/InsightsScreen.tsx` | Rebuilt with `AppScreen + AppForm` layout; includes `InsightsPeriodHeader`, `CategorySpendingList`, `TagSpendingList` |
| Both locale files | Add `insights.period` key |

---

## Reference

### Reusable utilities

| Utility | Import |
|---------|--------|
| `watchQuery` | `@/shared/persistence/watch-query` |
| `DateInterval.createFrom` | `@holefeeder/shared/core` |
| `Money.fromCents` / `Money.toCents` / `Money.ZERO` | `@holefeeder/shared/core` |
| `DefaultSettings` | `@/settings/core/settings` |
| `useSettings()` | `@/shared/presentation/core/use-settings` |
| `useRepositories()` | `@/shared/repositories/core/use-repositories` |

### Key reference files (read-only — do not modify)

| File | Why it matters |
|------|---------------|
| `src/dashboard/persistence/dashboard-repository-in-powersync.ts` | SQL + `watchQuery` pattern |
| `src/flows/persistence/flows-repository-in-powersync.ts` | Recursive CTE tag-splitting pattern |
| `src/dashboard/presentation/core/use-dashboard.ts` | Hook pattern to mirror |
