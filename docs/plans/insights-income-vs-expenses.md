# Insights — income vs expenses

Redesign of the mobile Insights tab around an income vs expenses line chart, with
spending and income details moved to their own pushed pages.

Design: Penpot file "Holefeeder Mobile", boards PROPOSAL 18B2 (Insights), 18D
(Spending), 18E (Income), light row y=10400 and dark row y=23300; prototype flows
"Insights proposal 18" and "Insights proposal 18 — Dark".

Each step below is one deliverable: it ends with tests green, the Maestro tags run, a
deploy to the phone (`/frontend:deploy-mobile`) and a review on the device before the
next step starts. Structural commits come first inside a step when needed.

## Decisions taken

- The chart excludes system categories (transfers). The mobile summary query already
  does (`system = 0`), so chart expenses equal the category-spending total.
- The Combined toggle belongs to the spending details only.
- Chart palette (validated for colour-vision deficiency):
  light — income `#2E7D32` solid, expenses `#FF8C42` dashed;
  dark — income `#26A69A` solid, expenses `#D96F26` dashed.
  Red/green failed the CVD check and is not used for the two lines.
- Chart rendering: a local Expo module in `modules/` wrapping Swift Charts
  (`LineMark`, `AreaMark(yStart:yEnd:)`, `chartXSelection`), same pattern as
  `modules/horizontal-scroll-view`. No new dependency. iOS only; Android hides the
  chart until it gets its own version later.
- When expenses exceed income the header badge reads "Overspent by {amount}".
- The range picker counts budget periods. Its labels come from a function that turns
  a period count and the user's budget settings (interval type, frequency) into text,
  e.g. monthly ×1, 12 periods → "1 year"; weekly ×2, 6 periods → "12 weeks";
  localized en-CA / fr-CA.
- The Income page has no Combined toggle (too few income entries to be useful).

## Steps

### 1. Spending page

Move today's Insights content to a pushed page; Insights keeps only a link to it.

- Structural: extract the period header, Combined toggle and category/tag lists from
  `InsightsScreen` into a `SpendingDetailsScreen` component, no behaviour change.
- Behavioural: new route `(app)/insights/spending` (pushed, like `accounts/[id]`,
  no tab bar), titled "Spending". Insights shows the current header and an
  `AppFieldLink` row "Spending this period — {current expenses}".
- Maestro: insights flows navigate through the new row.

On the phone: tap the row, see the old content on its own page, go back.

### 2. Net header on Insights

- Header becomes "Net this period" + signed net amount + a badge
  ("Saved 26% of income" / "Overspent by $580").
- Core: savings-rate calculation next to `calculateSummary`, unit-tested
  (zero income, negative net, rounding).
- Business rule added to `docs/business-rules/statistics.md`.

On the phone: Insights header shows this period's net.

### 3. Income page — categories

- Repository + use case for income by category (gain, non-system, current period and
  average over previous periods), mirroring category spending; PowerSync spec
  against a real database.
- Route `(app)/insights/income`, same layout as Spending: header "Current Income",
  vs-average badge with **inverted** meaning (up is good), "Income by Category".
- Insights gets "Income this period — {current gains}" above the spending row.

On the phone: both rows open their own page.

### 4. Income page — tags

- Income by tag repository/use case and list. No Combined toggle.

### 5. Period series (core only, no UI)

- Pure function building `{ period, gains, expenses }[]` for the last N budget
  periods from the existing `SummaryData` buckets, zero-filling empty periods.
- Unit tests: weekly and monthly intervals, frequency > 1, empty periods, current
  period last.

Small and invisible on purpose; it lands together with step 6 on the phone.

### 6. Chart v1 — two lines

- Structural first: scaffold the local Swift Charts module (`modules/`) and its
  `@expo/ui` mock, rendered nowhere.
- Two lines (income solid, expenses dashed), legend,
  period labels, fixed 6 periods, light and dark palettes from the theme. Not
  rendered on Android.
- Totals row under the chart (Income / Expenses / Net over the range) and the
  "Excludes transfers between your accounts" footnote.
- Accessibility: chart has a VoiceOver summary (e.g. "Income above expenses in 4 of
  6 periods").

On the phone: the chart with real data, both modes.

### 7. Surplus / deficit shading

- Fill between the lines: green where income is above, red where expenses are above,
  plus the "Deficit" legend entry.

### 8. Scrub to inspect a period

- Touch-and-drag selects a period: rule mark, highlighted points, tooltip with
  period, income, expenses and net.

### 9. Range picker

- Core: range-label function (period count + budget settings → localized text),
  unit-tested across weekly / monthly intervals, frequencies and both locales.
- Segmented control above the chart with three period counts labelled by that
  function; the series, totals and chart follow it. Persist the choice if it feels right on the phone.

### 10. Clean-up

- Remove anything the old Insights layout left unused, refresh the Penpot "— Light"
  / "— Dark" Insights boards to match the shipped app.
