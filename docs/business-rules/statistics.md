---
path: docs/business-rules/statistics.md
domain: statistics
last-reviewed: 2026-07
---

# Statistics Rules

## Concepts

Statistics are **read-only, derived views** over a user's transactions. They never
create, mutate, or store aggregated data — every figure is computed on demand from
the underlying transactions and categories. All figures are scoped to the
authenticated user.

There are two statistics surfaces:

- **Backend statistics** (`api/v2/.../statistics`) — served by the .NET API and
  consumed by the web app. Two endpoints: per-category yearly statistics and a
  period summary.
- **Mobile insights** — computed locally in the mobile app against the PowerSync
  SQLite mirror. Two views: category spending and tag spending, each pairing the
  current period's spend with an average over previous periods.

Both surfaces treat **only non-system, expense-type categories** as spending, and
only for the two spending views (category/tag). The summary endpoint reports gains
and expenses separately.

---

## Category Statistics (backend — yearly)

`GET api/v2/categories/statistics` — per expense category, spend broken down by year
and by month within each year.

**Rules:**

- Only **non-system** categories are included (`!category.System`).
- Only categories whose type **is an expense** appear in the result; gains are
  dropped.
- Transactions are grouped by category → year → month, then rolled up to a yearly
  total and a list of monthly totals.
- **`MonthlyAverage`** = total spend across all returned years ÷ total number of
  months that actually have transactions, rounded to 2 decimals. Months with no
  transactions are **not** counted in the denominator.
- Results are ordered by category name.
- A category with no transactions does not appear (inner join to transactions).

## Summary Statistics (backend — period)

`GET api/v2/summary/statistics?from={date}&to={date}` — gains vs. expenses across a
requested date range, reported as three data points.

**Rules:**

- `from` and `to` are both **required** — an empty/unparseable value fails
  validation.
- The interval type and frequency are **derived from the range** (`from`..`to`),
  not supplied by the caller.
- Transactions are summed per category type (Gain, Expense) per interval bucket.
- The response carries three `SummaryValue`s, each with a gain and an expense total:
  - **Last** — the period immediately *before* `from` (one iteration back).
  - **Current** — the period starting at `from`.
  - **Average** — mean of all buckets in range = Σ(bucket values) ÷ bucket count,
    rounded to 2 decimals; **0** when there are no buckets.
- A period with no matching transactions reports **0** (not missing).
- Unlike the category/tag views, the summary includes **system categories** (e.g.
  transfers contribute to gains/expenses).

---

## Category Spending (mobile insight)

Per expense category: how much was spent in the current period alongside the average
spend per previous period.

**Rules:**

- The **current period** is derived from the user's settings (effective anchor date,
  interval type, frequency) and the effective date passed in — the effective date is
  injected through the use-case, never read from `today()` inside the query.
- Only **non-system, expense** categories are considered
  (`c.system = 0 AND lower(c.type) = 'expense'`).
- **`spentAmount`** = sum of transaction amounts in the current period
  (`date >= start AND date <= end`).
- **`avgAmount`** = average spend over **previous complete periods** (see
  [Previous-period averaging](#previous-period-averaging)); `0` when there are no
  previous periods.
- Every matching category is returned even if it has no spend in any period (left
  joins), with amounts defaulting to `0`.
- Ordered by `avgAmount` desc, then `spentAmount` desc, then category name asc.
- `avgAmount` is rounded to whole cents in SQL; all amounts cross the boundary as
  `Money`.

## Tag Spending (mobile insight)

Same shape as category spending, but grouped by **tag** instead of category.

**Rules:**

- A transaction's `tags` column is a comma-separated string; it is split into
  individual tags. Blank tags and empty tag strings are ignored.
- Only transactions on **non-system, expense** categories contribute.
- **`spentAmount`** and **`avgAmount`** follow the same definitions as category
  spending.
- A tag that appears **only in previous periods** (no current spend) is still
  returned, with `spentAmount = 0` and its historical average.
- Ordered by `avgAmount` desc, then `spentAmount` desc, then tag asc.

---

## Previous-period averaging

Shared by the category- and tag-spending mobile insights.

- **Previous periods** are the complete budget periods walked forward from the
  settings anchor up to — but **excluding** — the current period's start. A period
  counts only when its exclusive end (the next boundary) does not pass the current
  start.
- For a `oneTime` interval there are **no** previous periods, so `avgAmount` is
  always `0`.
- The average divides total spend by the number of periods **from the item's first
  active period through the last previous period** — leading zero-spend periods
  (before the item's first transaction) are excluded from the denominator. This
  mirrors the backend `MonthlyAverage` rule of not counting empty periods.
- Period ranges are **start-inclusive, end-exclusive** (`date >= start AND
  date < end`) for previous periods; the current period is **inclusive on both
  ends** (`date >= start AND date <= end`).

---

## Invariants

- Statistics are always **computed, never stored** — there is no aggregate table or
  cached total.
- Every query is scoped to the owning user; cross-user data never appears.
- The category- and tag-spending views count **only non-system expense categories**;
  gains and system categories (e.g. transfers) are excluded.
- Averages never count leading empty periods in the denominator — the divisor starts
  at the first period with activity.
- A missing/empty period contributes **0**, never a null or a skipped row.