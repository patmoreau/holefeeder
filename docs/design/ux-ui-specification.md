# Holefeeder — UX/UI Specification

Source of truth for the **behaviour and visual rules** of the Holefeeder mobile app
(`frontend/apps/holefeeder-mobile`), written so that the planned **React 19 + Vite + MUI**
web app (`frontend/apps/holefeeder-web`) can be built from it without re-reading the
React Native code.

It also states, explicitly, **what is not covered by E2E tests** and **which links and
functionality are missing** in the current app, so the web build does not inherit the
same holes.

The web app's data layer is **plain REST against `api/v2`** — no PowerSync web SDK, no
TanStack Query. That decision and its full design are §11.

Derived from code as of `main` @ `d48239f9` (2026-08-29). Companion docs:
`docs/business-rules/` (domain rules, language-agnostic), `frontend/CLAUDE.md`,
`frontend/apps/holefeeder-mobile/CLAUDE.md`.

---

## 1. Product model, in UI terms

| Concept | What the user sees | Where it lives |
|---|---|---|
| **Account** | A card with a name, current balance, projected balance, last-updated date | Dashboard carousel, Account detail |
| **Transaction** | A row: icon, description, tags, amount, date | Dashboard "Recent Transactions", Account detail "Transactions" |
| **Cashflow** | A recurring bill/income template: amount, cadence, category, tags | Settings → Manage Cashflows |
| **Upcoming flow** | A projected, not-yet-paid occurrence of a cashflow | Dashboard "Upcoming Flows" |
| **Category** | Name + colour + type (expense/gain) + optional budget | Settings → Manage Categories, pickers |
| **Tag** | Free-form lowercase label on transactions/cashflows | Chips in forms, Settings → Manage Tags |
| **Budget period** | Effective date + interval type + frequency; defines "this period" | Settings → Budget Settings, onboarding step 1 |
| **Summary / insights** | Current-period spending, variation vs average, breakdown by category/tag | Dashboard header, Insights tab |

Invariants that the UI must respect (full detail in `docs/business-rules/`):

- **All stored amounts are positive.** Sign is presentation: `accountTypeSign × categoryTypeSign`.
  Debit accounts (checking, investment, savings) = `+1`; credit accounts (credit card,
  credit line, loan, mortgage) = `−1`. Gain = `+1`, expense = `−1`.
- **Balance is always computed, never stored.**
- **Soft delete** for accounts (`inactive`) and cashflows; **hard delete** for transactions.
- **Categories are never hard-deleted** — existing transactions must keep resolving.
- **Tags** are lowercased, de-duplicated, blanks discarded, and inherited by transactions
  created from a cashflow.
- Everything is **user-scoped**; all reads/writes filter on the authenticated user.

---

## 2. Information architecture

### 2.1 Route map (mobile)

```
_layout (providers: Language → Theme → CombinedInsight → Auth → PowerSyncAuth → Repository → Registration)
└── HolefeederContent  — three mutually exclusive guarded groups
    ├── (auth)          guard: no user
    │   └── Welcome
    ├── (onboarding)    guard: user && not registered
    │   ├── Registering      (auto-advances)
    │   ├── BudgetPeriod
    │   ├── FirstAccount
    │   └── Categories
    └── (app)           guard: user && registered
        ├── (tabs)
        │   ├── index        Dashboard
        │   ├── statistics   Insights
        │   └── settings     Settings
        ├── Purchase                  push,  ?accountId
        ├── accounts/[id]             transparentModal — Account detail
        ├── flows/[id]                transparentModal — Edit transaction
        ├── AddAccount / EditAccount  push
        ├── AddCategory / EditCategory push
        ├── EditCashflow              push
        ├── BudgetSettings            modal
        ├── ManageCategories          modal
        ├── ManageCashflows           modal
        ├── ManageTags                modal
        ├── SyncSettings              modal
        └── PayUpcoming               modal, ?data=<serialized UpcomingFlow>
    └── +not-found, help
```

**Global affordance:** a `purchase` toolbar button (cart icon) sits on the right of the tab
bar header on every tab. iOS quick actions (long-press app icon) offer *Purchase* and *Help*.

### 2.2 Navigation rules

- **Gate order matters.** Signed in but registration status unknown → full-screen spinner.
  Registration check failed → `RegistrationError` with Retry (never guess a side of the gate).
- `Registering` uses `router.replace`, not push — it must not be reachable via Back.
- Forms navigate back on save (`goBack()`), never forward.
- Account detail → Purchase carries `accountId` so the purchase pre-selects that account.
- `+not-found` offers a single button back to Dashboard.

---

## 3. Design tokens

Defined in `src/types/theme/`. These are the numbers the web app should adopt verbatim
unless a web-specific reason overrides them.

### 3.1 Colour

| Token | Light | Dark | Use |
|---|---|---|---|
| `primary` | `#7B42F6` | `#9D6DE6` | Brand purple. Headers, primary buttons, tint |
| `secondary` | `#FF8C42` | `#FF9E66` | Accent orange (currently barely used) |
| `background` | `#FFFFFF` | `#000000` | Page ground |
| `secondaryBackground` | `#F2F2F7` | `#1C1C1E` | Cards, grouped list ground |
| `text` | `#1C1C1E` | `#F2F2F7` | Body |
| `primaryText` | `#FFFFFF` | `#F2F2F7` | Text **on** primary (header content) |
| `secondaryText` | `#6C6C70` | `#8E8E93` | Labels, captions |
| `destructive` | `#E53B3B` | `#FF453A` | Delete actions |
| `error` | `#FF3B30` | `#FF453A` | Field/system errors |
| `separator` | `#C6C6C8` | `#38383A` | Hairlines, dashed borders |
| `link` / `tint` | `#7B42F6` | `#9D6DE6` | Links, active tab |
| `tabIconDefault` | `#687076` | `#AEAEB2` | Inactive tab |
| `positive` / `positiveBackground` | `#2E7D32` / `#E6F7ED` | `#7CB342` / `#1E3A2E` | Under budget, gain, credit |
| `negative` / `negativeBackground` | `#D32F2F` / `#FFF2F2` | `#FF6F42` / `#3A1E1E` | Over budget, expense, debit |
| `amountNeutral` | `#1C1C1E` | `#F2F2F7` | Amount with no sign meaning |
| `dashboard` / `accounts` / `settings` | `#F0E6F7` / `#FFF5EB` / `#F2F2F7` | `#2E1A47` / `#472F1A` / `#1C1C1E` | Section tints (declared, largely unused) |

**Semantic rule:** positive/negative are *not* the accent. They encode budget and flow
direction only, and always appear as a **pill** — coloured text on the matching
`*Background`, `borderRadius.full`, `spacing.sm` horizontal / `spacing.xs` vertical.

### 3.2 Spacing — 4px grid

`xs 4 · sm 8 · md 12 · lg 16 · xl 20 · 2xl 24 · 3xl 32 · 4xl 40 · 5xl 48`

### 3.3 Radius

`none 0 · xs 2 · sm 6 · md 8 · lg 10 · xl 12 · 2xl 16 · 3xl 20 · 4xl 24 · full 9999`

- Cards: `xl` (12)
- Grouped section containers: `3xl` (20)
- Coloured headers (dashboard/account/insights banners): `4xl` (24) on the bottom corners
- Pills/chips: `full`

### 3.4 Typography

iOS scale (web should use the `default` scale):
`xs 11 · sm 12 · base 14 · md 16 · lg 18 · xl 22 · 2xl 30 · 3xl 36 · 4xl 40`

| Variant | Size | Weight | Used for |
|---|---|---|---|
| `display` | `4xl` | 700 | Current-spending number in dashboard/insights header |
| `largeTitle` | `3xl` | 700 | Account balance |
| `title` | `xl` | 600 | Screen/card titles |
| `subtitle` | `md` | 400 | Header labels |
| `body` | `lg` | 400 | Default text |
| `secondary` | `md` | 400 | Secondary text |
| `footnote` | `sm` | 400 | Dates, avg per period, counts |
| `chip` | `base` | 400 | Tag chips |
| `errorField` | `sm` | — | Inline field errors |

Line heights: `tight 1.2 · snug 1.375 · normal 1.5 · relaxed 1.625 · loose 2`.
The amount input is a special case: **48px, weight 600, centred**, tinted by tone.

### 3.5 Elevation, sizing, misc

- Shadows `sm/base/md/lg/xl` — cards use `base` (`0 2px 4px @ 0.1`); headers use the same.
- Min touch target 44 (iOS) / 48 (Android). Buttons `sm 32 · md 44 · lg 52`. Inputs `sm 36 · md 40 · lg 48`.
- Icon sizes `xs 16 · sm 20 · md 24 · lg 32 · xl 40`.
- Opacity: `disabled .38 · hover .08 · focus .12 · selected .16 · pressed .32`.
- Z-index: `dropdown 10 · sticky 20 · overlay 30 · modal 40 · popover 50 · toast 60 · tooltip 70`.

---

## 4. Component vocabulary

The mobile app funnels **all** native UI through `src/shared/presentation/components/native/`
(`App*` wrappers over `Expo*` passthroughs over `@expo/ui`). Feature code never imports
`@expo/ui` directly — an ESLint rule enforces it. The web app should keep the same discipline
with a single `components/` layer over MUI, so screens stay platform-agnostic.

| Mobile wrapper | Role | Web/MUI equivalent |
|---|---|---|
| `AppScreen` / `AppView` | Page container, offsets header height | `Container` + layout shell |
| `AppForm` | Grouped, scrollable form body | `Stack` inside a `Paper`/`Card` |
| `AppFieldGroup` / `AppFieldSection` | iOS grouped-list section with optional title | `Card` + `CardHeader` + `List` |
| `AppField` | Label + icon + control + inline error, one row | `ListItem` with `ListItemIcon`, `ListItemText`, secondary action |
| `AppTextInput` | Text entry | `TextField` |
| `AppPicker` | Wheel/menu single-select | `Select` / `Autocomplete` |
| `AppMenu` | Rich menu with icons per item (used for Category) | `Menu` + `MenuItem` with colour dot |
| `AppSegmentedMenu` | Segmented control (Expense/Income/Transfer) | `ToggleButtonGroup` |
| `AppSwitch` | Boolean | `Switch` |
| `AppDatePicker` | Date | MUI X `DatePicker` |
| `AppColorPicker` | Category colour | Colour swatch grid / `<input type=color>` |
| `AppChip` | Tag chip, selected state | `Chip` (`filled`/`outlined`) |
| `AppButton` | `primary` / `secondary` / `destructive` / `link` variants, optional icon + position | `Button` `contained`/`outlined`/`error`/`text` |
| `AppListItem` (+ `.Leading`, `.Trailing`, `.Supporting`) | Row with icon, trailing column, supporting row | `ListItem` composition |
| `AppSwipeActions` | Leading/trailing swipe actions, optional full-swipe | **No web equivalent** — see §6.4 |
| `AppBottomSheet` | Half-height sheet (rename tag) | `Dialog` / `Drawer anchor="bottom"` |
| `AppToolbar` / `AppToolbarButton` | Header actions, `placement="left|right"` | `AppBar` actions / page header buttons |
| `AppErrorSheet` | Error surface with Retry / Dismiss | `Alert` + `Snackbar`, or error state panel |
| `AppLoadingIndicator` | Spinner, `size`, `withBackground` | `CircularProgress` / `Skeleton` |
| `AppProgressView` | Infinite-scroll sentinel (`onAppear`) | `IntersectionObserver` sentinel |
| `AppCard` / `AppCardList` | Horizontal card carousel | `Card` in a scroll/grid container |
| `ExpenseTrendBadge` | Trend pill: icon + amount or % | Custom `Chip` |

**Icons.** Never raw strings — `AppIconMap.<key>` maps one key to an SF Symbol (iOS) and a
Material Symbol (Android). The web app inherits the **same keys** and binds them to
`@mui/icons-material`, so screen code stays identical: `account, accounts, add, addCircle,
back, calendar, cancel, cashflow, category, close, circle, combined, connected, dashboard,
delete, description, dropdown, edit, download, expand, expiresAt, favorite, frequency,
insights, language, purchase, menu, uploadOutstanding, save, selected, settings, share,
storeItem, sync, tag, theme, token, trendUp, trendDown, upload, warning`.

---

## 5. Screen specifications

Each entry: purpose → data → layout → states → actions. `testID` values are listed because
they are the E2E contract and should be mirrored (as `data-testid`) on web.

### 5.1 Welcome — `(auth)/Welcome` · `welcome-screen`

Centred column: title *"Track where your money goes"*, subtitle, **Create an account**
(primary, `welcome-signup-button`), **Sign in** (secondary, `welcome-signin-button`).
Both open the same Auth0 flow; signup passes `screen_hint=signup`.
While auth resolves: large spinner, nothing else.

### 5.2 Onboarding

Four steps, headers transparent, the action is **a row inside the form** — not a toolbar
button — because rows are hit-testable and carry testIDs.

1. **Registering** (`onboarding-registering-screen`) — title, subtitle, spinner. Auto
   `router.replace('/BudgetPeriod')` when ready. On failure:
   `onboarding-registering-failed` + Retry (`onboarding-registering-retry-button`).
2. **BudgetPeriod** (`onboarding-budget-period-screen`) — *"When does your budget start?"*.
   Fields: effective date, interval type, frequency. Footer row **Continue**
   (`onboarding-budget-period-continue-button`). Defaults seed the form but are still
   written on save, so the app never runs on values nobody chose.
3. **FirstAccount** (`onboarding-first-account-screen`) — reuses `EditAccountFormContent`
   with `id === null`, which **hides the Inactive switch** (a new account must not be
   creatable switched off). Name input `account-name-input`. Footer **Finish**
   (`onboarding-first-account-finish-button`). Favourite defaults to true.
4. **Categories** (`onboarding-categories-screen`) — *"What do you spend on?"*, eight
   suggestions (groceries, restaurants, transportation, housing, utilities, health,
   entertainment, shopping), **all pre-selected**, each a switch
   (`onboarding-category-<key>-switch`). **Finish** and **Skip for now** are both real
   answers. On failure a warning row appears above the actions.

### 5.3 Dashboard — `(tabs)/index` · `dashboard-screen`

**Layout:** a primary-purple header block pinned to the top (height = ⅓ viewport,
`borderRadius 4xl`), with the scrolling list underneath, top-padded to `headerHeight − 125`
so the account cards overlap the header.

**Header content** (`DashboardHeaderLargeCard`):
- "Current Spending" label + the period total in `display` type, white.
- `ExpenseTrendBadge` — *"{amount} vs Average"*, trend-up icon + negative colours when over,
  trend-down + positive when under.
- Hairline divider (white @ 20%).
- Two pill stats side by side: **Net Flow** (actual) and **Projected** (net flow +
  Σ upcoming × category sign). Sign is rendered as an explicit `+ ` / `- ` prefix.

**Body:**
1. **Account carousel** — horizontal, cards 300×190. Each card: name, current balance
   (`largeTitle`), then a two-column footer above a hairline: *Updated* + relative date,
   *Projected* + amount tinted positive/negative by `balance × accountTypeSign`. Per-card
   balance loads independently (spinner inside the card). Last item is the **Add account**
   card — dashed border, transparent, plus-circle icon (`dashboard-add-account-card`).
   Order: **favourites first, then name**; inactive accounts excluded.
2. **Recent Transactions** — 3 most recent, hidden entirely when empty. Row → `flows/[id]`.
3. **Upcoming Flows** — every projected occurrence in the period. Row → `PayUpcoming`.
   Swipe **leading**: Pay (full swipe allowed). Swipe **trailing**: Delete (destructive,
   confirmation alert), Clear (secondary).

**Error state:** whole screen replaced by `AppErrorSheet`.

### 5.4 Account detail — `(app)/accounts/[id]` (transparent modal)

Purple header block (bottom corners `4xl`): account name (`title`), balance (`largeTitle`),
divider, then two centred stats — *Updated* (date pill, always positive colours) and
*Projected* (pill tinted by sign, plus a small `± upcoming variation` line when non-zero).

Below: **Transactions** section, 50 per page, **infinite scroll** via an `onAppear`
sentinel keyed on the loaded count. Newest first (`date DESC, id DESC`).
Row swipe **trailing**: Delete, full-swipe allowed, confirmation alert first.

Toolbar: left = Back; right = an overflow **Menu** (native primitive, no testID, invisible to
Maestro) with **Edit** and **Purchase**.

Loading: spinner. Missing account: spinner forever (see §9).

### 5.5 Purchase — `(app)/Purchase` · `purchase-screen`

The app's primary write path. Toolbar: Save (`purchase-save-button`), Back
(`purchase-back-button`).

- **Segmented control**: Expense · Income · Transfer.
- **Amount** — 48px centred input, autofocused, digit-only entry interpreted as cents
  (typing `1234` → `12.34`), reformatted through the locale formatter on every keystroke,
  caret forced to the end. Tone: expense = negative colour, income = positive, transfer = neutral.
- **Expense/Income**: Date · Account (picker) · Category (colour-dot menu, filtered to
  matching type, auto-selects the first when the current one no longer fits) · Tags · Note.
- **Transfer**: Date · Source · Target · Note. Choosing the same account as source fires an
  **error haptic** immediately and fails validation with *"Transfer destination must be
  different from source account"*.
- **Cashflow details** (non-transfer only): a switch; when on, reveals effective date,
  interval type, frequency.

Defaults: type = expense, date = today, amount = 0, source = the passed `accountId` or the
first account, target = any other account, category = first, cashflow off, monthly ×1.
Because a category is pre-selected the form is **dirty on open**, so backing out always
raises the discard alert.

**Tag entry** deserves its own note: chips are ordered by *count within the current
category*, then total count, then alphabetically, with selected tags pinned first. The
filter field is a live filter; submitting it selects the single match, or creates a new tag
from the typed text.

### 5.6 Edit transaction — `(app)/flows/[id]`

Same shape as Purchase minus transfer and cashflow: segmented Expense/Income, amount, date,
account, category, tags, note. Save `flow-save-button` / Back `flow-back-button`.

### 5.7 Pay upcoming — `(app)/PayUpcoming` (modal)

Amount (pre-filled from the cashflow), date. **When the amount is changed**, an extra row
appears: *"Update recurring amount to {amount} going forward"*. Reached by tapping an
upcoming row, or bypassed entirely by the swipe **Pay** action.

### 5.8 Add/Edit account — `AddAccount` · `EditAccount`

Sections: (1) Name + Type picker (checking, credit card, credit line, investment, loan,
mortgage, savings); (2) Opening balance (48px amount field) + Opening date; (3) Note;
(4) Favourite switch, and **Inactive switch only when the account already exists**.
Validation: name required. Save `edit-account-save-button`, Back `edit-account-back-button`.

### 5.9 Insights — `(tabs)/statistics`

Purple banner: period label (formatted date range), "Current Spending", the total in
`display`, and the trend badge. Then:

- **Combined** switch (persisted in app storage, provided app-wide by `CombinedInsightProvider`).
- Off → **Spending by Category** + **Spending by Tag**.
- On → **Spending by Category & Tag** (each category row followed by indented tag rows).

Category row: colour dot, name, spent amount — tinted **negative when over budget**,
**positive when under**, neutral when no budget. Under budget also shows `NN%` of budget;
over budget shows *"Over budget"*. An average line (*"avg {amount}"*) appears when the
average over previous periods is non-zero. Ordering: `avgAmount DESC, spentAmount DESC, name ASC`.
Empty: *"No expenses this period"* (per section).

### 5.10 Settings — `(tabs)/settings`

Grouped sections, each a single navigation row with a chevron:
**User Profile** (avatar 120px circle with a primary-coloured 3px ring, falling back to a
generated `ui-avatars.com` image, name, email, Logout `auth-logout-button`) ·
**Holefeeder → Budget Settings** · **Categories → Manage Categories** ·
**Cashflows → Manage Cashflows** · **Tags → Manage Tags** · **Display** (Language:
English/French; Theme: Light/Dark/Automatic) · **Test** (dev builds only) ·
**Synchronization → Statistics**.

Sub-screens:

- **Budget Settings** (modal) — effective date, interval type, frequency.
  `budget-settings-save-button` / `budget-settings-back-button`.
- **Manage Categories** (modal) — list ordered favourites-first then name; colour dot,
  name, chevron. **System categories are not editable** (no press handler, no swipe delete,
  trailing text "System"). Toolbar **+** → Add Category. Swipe trailing Delete (confirm alert,
  soft delete). Category form: name, type, colour picker, budget amount, favourite.
- **Manage Cashflows** (modal) — rows show description (or category name when blank),
  amount, cadence (`3 × Monthly`), tags, category colour. Ordered by effective date DESC.
  Swipe trailing Delete, **full-swipe, no confirmation** — soft delete is optimistic and the
  row reappears if the write fails. Row → Edit Cashflow (amount, account, category, tags,
  note, effective date, interval type, frequency, recurrence 0–24 where 0 = "Never ends").
- **Manage Tags** (modal) — tag name, transaction count, active-cashflow count. Tap → rename
  bottom sheet (half snap point, Cancel/Save, Save disabled while empty). Swipe trailing
  Delete with confirmation. Empty state: *"No tags yet"*.
- **Synchronization** (modal) — read-only diagnostics: connected, last synced, downloading,
  uploading, not-synced count, and row counts per table; plus **Share** which copies the
  SQLite file and opens the share sheet.

---

## 6. Interaction rules

### 6.1 Form contract

Every form is built on `createFormDataContext(displayName, saveFn)`, which supplies dirty
tracking, per-field errors, general errors, validation, save, and an error sheet.
`useFormActions` then defines the **universal save/cancel behaviour**:

- **Save**: if not dirty → just go back. If dirty → save; on failure show a **Form Errors**
  alert listing the count (and, in dev builds only, the field/message pairs); stay on the form.
  On success → go back.
- **Cancel/Back**: if not dirty → go back. If dirty → **Discard changes?** alert
  (*"You have unsaved changes…"*, Discard / Stay), destructive styling on Discard.
- Validation runs on change (`validateOnChange`) for Purchase, Flow and onboarding forms.

### 6.2 Alerts (native `Alert.alert`)

| Alert | Title | Buttons |
|---|---|---|
| Delete | *Delete {item}* / *Do you want to delete this {item}?* | Cancel · **Delete** (destructive) |
| Discard | *Discard changes?* | Stay · **Discard** (destructive) |
| Form error | *Form Error(s)* (pluralised) | Dismiss |

### 6.3 Async states — the three-way rule

Everything reactive returns `Result<T>` = `Loading | Success | Failure`, and every screen
must render all three:

- **Loading** → `AppLoadingIndicator` (screen-level) or an in-card spinner (account cards).
- **Failure** → `AppErrorSheet` with **Retry** / **Dismiss**; on the dashboard it replaces
  the whole screen.
- **Success + empty** → each list defines its own empty copy; *Recent Transactions* is the
  exception and renders nothing at all when empty.

`useMultipleWatches` + `withDefault` combine several watches into one `{ data, isLoading, errors }`.

### 6.4 Swipe actions — inventory (this is the biggest port problem)

| Row | Leading | Trailing | Full swipe | Confirm |
|---|---|---|---|---|
| Upcoming flow | **Pay** (primary) | **Delete** (destructive), **Clear** (secondary) | leading only | Delete only |
| Transaction | — | **Delete** | yes | yes |
| Cashflow | — | **Delete** | yes | **no** (optimistic soft delete) |
| Category | — | **Delete** (hidden for system categories) | no | yes |
| Tag | — | **Delete** | no | yes |

There is no web analogue. See §10.3 for the recommended replacement.

### 6.5 Formatting

- Currency, dates, date ranges and percentages all go through `LocalFormatter` with the
  active locale and currency code from `useLocaleFormatter()`.
- Dates are **relative when near**: Today, Yesterday, *{n} days ago* (≤7), Tomorrow,
  *in {n} days* (≤7), otherwise an absolute date.
- Amount entry is **cents-first**: only digits are accepted, value = digits / 100.
- Signs are rendered as text prefixes (`+ ` / `- `), not by negative numbers.

### 6.6 i18n

Every user-facing string comes from `useTranslation()`; keys live in
`packages/shared/src/core/translations/locales/{en-CA,fr-CA}/`. The web app must consume
the **same** package — the key tree is already platform-neutral. Language is user-selectable
and persisted; the tab bar is re-keyed on language change so labels re-render.

### 6.7 Offline & sync

PowerSync owns the local SQLite mirror; all reads are local watches, so **every list is live
and every write is optimistic**. The connection lifecycle belongs solely to
`PowerSyncAuthProvider` (connect on user, disconnect on sign-out). The UI never shows a
blocking "saving" state — this is the behaviour the web app must reproduce, and the reason a
plain REST web client would feel different.

### 6.8 Accessibility

- `accessibilityLabel` on every icon-only button; `accessibilityRole="button"` on custom pressables.
- Minimum touch target 44px.
- `adjustsFontSizeToFit` on amounts so long currency values never truncate.
- Colour is never the only signal: over-budget also prints *"Over budget"*, trends also carry
  an arrow icon.

---

## 7. E2E coverage (Maestro)

Flows in `frontend/apps/holefeeder-mobile/.maestro/`. Elements are selected by `id:`
(the `testID`), never by visible text, because text is translated. Three tags, each needing
a different build: `regression` (injected session), `auth` (real Auth0), `onboarding`
(second, never-registered Auth0 user, reset before each run).

### 7.1 What is covered

| Flow | Tag | Asserts |
|---|---|---|
| `auth/login` | auth | Welcome → real Auth0 → dashboard renders (proves the token reached PowerSync) |
| `auth/logout` | auth | Logout clears the Auth0 session, next sign-in asks for an email |
| `auth/signup` | auth | Create-account opens Auth0 **on the signup page** (`screen_hint` honoured) |
| `auth/signup-cancelled` | auth | Cancelling Auth0 returns to Welcome with both buttons intact |
| `dashboard/opens-dashboard` | regression | Injected session lands on the dashboard |
| `toolbar/form-actions` | regression | Purchase opens, Back raises **Discard changes?**, Discard returns to dashboard |
| `onboarding/gate` | onboarding | Unregistered user reaches onboarding, never the dashboard |
| `onboarding/budget-period` | onboarding | Continue writes the period and advances to first account |
| `onboarding/first-account` | onboarding | Inactive switch absent; named account saves and advances to categories |
| `onboarding/categories` | onboarding | Deselecting two categories and finishing lands on the dashboard |
| `onboarding/register` | onboarding | Onboarding is asked once — a second launch goes straight in |

### 7.2 What is **not** covered — by screen

| Area | Covered? | Gap |
|---|---|---|
| Purchase — **saving an expense** | ❌ | The single most important write path is only tested by *cancelling* it |
| Purchase — income, **transfer**, same-account validation | ❌ | Including the haptic/error path |
| Purchase — amount field (cents entry, formatting, tone) | ❌ | No test at any level of the 48px input |
| Purchase — tag selection / tag creation | ❌ | Ordering and create-on-submit untested |
| Purchase — cashflow toggle and recurring creation | ❌ | |
| Edit transaction (`flows/[id]`) | ❌ | Screen has no screen-level testID |
| Delete transaction (swipe + confirm) | ❌ | Swipe actions are untested everywhere |
| Account detail screen | ❌ | No testID; header, pagination and the overflow menu untested |
| Infinite scroll / load more | ❌ | |
| Add / Edit account outside onboarding | ❌ | Only the onboarding variant runs |
| Insights tab (any of it) | ❌ | Including the Combined toggle and the persisted preference |
| Settings screen and every sub-screen | ❌ | Budget settings, categories, cashflows, tags, sync |
| Manage Categories — system-category protection | ❌ | A real invariant with no test |
| Rename tag bottom sheet | ❌ | |
| Pay upcoming (row tap **and** swipe Pay) | ❌ | |
| Theme switching (light/dark) | ❌ | |
| **Language switching (en ↔ fr)** | ❌ | Nothing verifies the French app renders |
| Offline behaviour / sync recovery | ❌ | |
| Error states (`AppErrorSheet`, registration failure) | ❌ | `registration-error` has a testID but no flow |
| Deep links & quick actions | ❌ | Only the E2E auth link is exercised |

### 7.3 Structural gaps in the test setup

- **Native toolbars are invisible to Maestro.** Header actions only work in E2E builds
  because they are re-rendered as ordinary views via `headerRight`; the native
  `Stack.Toolbar.Menu` on the account screen cannot be automated at all.
- **Screens lacking a screen-level testID:** account detail, edit transaction, insights,
  settings, all four manage screens, sync settings, pay upcoming.
- **Onboarding flows are one-at-a-time** and depend on `reset-new-user.sh` wiping backend
  rows; three of them still carry a "NOT RUNNABLE YET" comment.
- E2E is **not part of CI** — a booted simulator and the local Docker stack are required.

---

## 8. Coverage matrix — feature × surface

| Capability | UI exists | Unit-tested | E2E | Notes |
|---|---|---|---|---|
| Sign in / sign up / sign out | ✅ | partial | ✅ | Strongest area |
| Registration gate | ✅ | ✅ | ✅ | |
| Onboarding (3 steps) | ✅ | ✅ | ✅ | |
| Dashboard summary + trend | ✅ | ✅ (hooks) | render only | Numbers never asserted |
| Account carousel | ✅ | ✅ (hooks) | ❌ | |
| Account detail + pagination | ✅ | ✅ (hooks) | ❌ | |
| Create purchase / income / transfer | ✅ | ✅ (form hooks) | ❌ | |
| Edit / delete transaction | ✅ | ✅ | ❌ | |
| Cashflows CRUD | ✅ | ✅ | ❌ | |
| Pay / clear / delete upcoming | ✅ | ✅ | ❌ | |
| Categories CRUD + system protection | ✅ | ✅ | ❌ | |
| Tags rename / delete | ✅ | ✅ | ❌ | |
| Budget period settings | ✅ | ✅ | ✅ (onboarding only) | |
| Insights (category / tag / combined) | ✅ | ✅ (repository) | ❌ | |
| Theme + language | ✅ | ✅ | ❌ | |
| Sync diagnostics + DB share | ✅ | partial | ❌ | |

Jest coverage is enforced at **70%** for branches, functions, lines and statements.
React Native UI components are deliberately not unit-tested; hooks are.

---

## 9. Missing links and functionality in the current app

Found while reading the code — these are product gaps, not test gaps.

**Navigation dead ends**

1. **No accounts list.** Accounts are reachable *only* through the dashboard carousel.
   With many accounts the horizontal scroll is the only way in, and the tab bar has no
   Accounts entry (though `tabs.accounts` and an `accounts` colour token both exist).
2. **No way to see or reopen inactive accounts.** The query filters `inactive = 0` and no
   screen lists them, so switching an account to Inactive removes it permanently from the UI.
3. **No account deletion** — only soft-inactivation via the edit form.
4. **`cardList.viewAll` ("View All") is translated but has no UI.** The card lists have no
   "see all" affordance, which is the natural fix for gap 1.
5. **`accountDetail.loadMore` is translated but unused** — pagination is sentinel-driven,
   with no manual fallback if the sentinel never fires.
6. **Account detail renders a spinner forever** when the id does not resolve; there is no
   not-found state for a deleted or foreign account.
7. **`help.tsx` is placeholder content** ("This is a modal") — yet it is a shipped iOS quick
   action titled *"Wait! Don't delete me!"*.
8. **`home.*` translations are Expo starter leftovers** and reference `app/(tabs)/index.tsx`
   edit instructions — dead copy in a shipped locale file.

**Functional gaps**

9. **Insights has no period navigation.** The header hard-codes offset `0`
   (`DateInterval.createFrom(today(), 0, …)`), so previous periods cannot be viewed even
   though the averaging logic already computes across them.
10. **No charts anywhere.** Insights is entirely list-based; `dashboard.largeHeader.avgSpending`
    and `totalIncome` keys exist but nothing renders them.
11. **No search or filter over transactions** — not by text, date range, category, tag or amount.
12. **No transaction detail view** — tapping a transaction goes straight to the edit form.
13. **Transfers cannot be edited.** `flows/[id]` offers only Expense/Income; a transfer is two
    linked transactions and editing one would break the pair.
14. **The dashboard header animation is inert.** `headerHeight` is an `Animated.Value` that is
    created and never driven — the collapse-on-scroll behaviour is scaffolded but not implemented.
15. **No budget-vs-actual view per period other than the category list**, and no notification
    or badge when a category goes over budget.
16. **No export** beyond sharing the raw SQLite file from the sync screen.
17. **No empty/first-run state on the dashboard** for a user who skipped categories and has no
    transactions — the sections simply render empty or vanish.
18. **`secondary` (orange) is defined in both themes but essentially unused**, as are the
    `dashboard`/`accounts`/`settings` section tints — the visual system is narrower than its tokens.

---

## 10. Porting to React + Vite + MUI

**Target stated by the author: a web app that feels like a web app, not a phone app in a
browser.** That means the rules above split into two piles — domain/behaviour rules that
port verbatim, and mobile idioms that must be *re-designed*, not translated.

### 10.1 Ports verbatim

Design tokens (§3) · the domain model and sign rules (§1) · the `Result` three-state
contract (§6.3) · form save/cancel/dirty semantics (§6.1) · formatting rules (§6.5) ·
the shared i18n package (§6.6) · the icon key vocabulary (§4) · `@holefeeder/shared/core`
in full — it is already platform-agnostic (no React Native, no Expo, no browser APIs), and
only needs `Id.setGenerator(crypto.randomUUID)` at bootstrap.

### 10.2 Layout: from stack navigation to a desktop shell

| Mobile | Web |
|---|---|
| Bottom tab bar (Dashboard / Insights / Settings) | Persistent left **navigation rail**; collapses to a bottom bar under `sm` |
| Full-screen push (Purchase, Edit account…) | **Right-hand drawer or dialog** — the list stays visible behind it |
| `presentation: 'modal'` (Budget, Manage *, Sync) | **Dialog** for short forms; a real route for Manage screens |
| `transparentModal` (account detail) | A **route** — `/accounts/:id`, a page in its own right |
| Header toolbar Save/Back | Dialog footer actions (Cancel / Save) or a sticky page action bar |
| One column, header block ⅓ of the viewport | **Two/three-column grid**: summary rail + content; the purple header becomes a summary card, not a full-bleed banner |
| Horizontal account carousel | **Responsive card grid** with an "all accounts" page — closes gap §9.1 |
| Amount 48px centred | Keep it large in the drawer; it is a strong signature. Add keyboard-friendly entry (accept `.`/`,` and paste), keeping cents-first as the fallback |

Suggested route table:

```
/                     Dashboard
/accounts             Accounts (new — fixes gap 1)
/accounts/:id         Account detail
/transactions         Transactions (new — search/filter, fixes gap 11)
/insights             Insights
/settings             Settings (nested tabs: budget, categories, cashflows, tags, display, sync)
/purchase             Drawer over the current route
```

### 10.3 Mobile idioms that need a web answer

| Idiom | Web replacement |
|---|---|
| **Swipe actions** (§6.4) | Row hover actions + an overflow `IconButton` menu; keep the same verbs (Pay / Clear / Delete). Full-swipe-without-confirmation on cashflows must become an explicit action **with undo** (`Snackbar` + Undo), which is the web equivalent of an optimistic full swipe |
| Native `Alert.alert` | MUI `Dialog` for destructive confirms; `Snackbar` for transient results. Do **not** use `window.confirm` |
| Bottom sheet (rename tag) | `Dialog` on desktop, `Drawer anchor="bottom"` on mobile widths |
| Wheel pickers | `Select` for short lists, `Autocomplete` for accounts/categories once counts grow |
| `onAppear` sentinel | `IntersectionObserver`, or a paginated table with page size control |
| Haptics (same-account transfer) | Inline field error + shake/`aria-live` announcement |
| Quick actions | Keyboard shortcuts (`N` = new purchase) and a `Fab`/toolbar action |
| PowerSync live queries | **Decided: plain REST**, no PowerSync web SDK, no TanStack Query. Full design in §11 |

### 10.4 MUI theme mapping

```
palette.primary.main        = primary            (#7B42F6 / #9D6DE6)
palette.secondary.main      = secondary          (#FF8C42 / #FF9E66)
palette.background.default  = background
palette.background.paper    = secondaryBackground
palette.text.primary        = text
palette.text.secondary      = secondaryText
palette.error.main          = error
palette.success.main        = positive
palette.divider             = separator
shape.borderRadius          = 12                 (borderRadius.xl)
spacing                     = 4                  (the 4px grid: theme.spacing(4) = 16px)
```

Typography variants map `display → h1`, `largeTitle → h2`, `title → h6`, `subtitle → subtitle1`,
`body → body1`, `footnote → caption`. Positive/negative pills become a single `AmountPill`
component so the semantic pair is never hand-rolled per screen.

### 10.5 Suggested build order

1. **Shell**: providers (language, theme, auth, data), navigation rail, routes, MUI theme
   from §10.4, the `App*` → MUI component layer from §4, and the REST infrastructure of §11.2.
2. **Read paths**: Dashboard, Accounts, Account detail, Insights — proves the store and
   invalidation design before any write exists.
3. **Write paths**: Purchase (expense → income → transfer), edit transaction, pay upcoming —
   with the form contract from §6.1 implemented once, centrally.
4. **Management**: cashflows, budget settings, sync — then categories and tags, each of which
   carries a backend slice of its own (§11.5), written when that screen is built.
5. **Web-only wins** the mobile app lacks: accounts list, transactions search, period
   navigation in Insights, charts, export.

Write E2E (Playwright) **as each slice lands**, starting with the paths §7.2 lists as
uncovered — the web app is the chance to have the purchase-save path tested before the
purchase-cancel path.

### 10.6 Decisions to make before coding

1. ~~Data layer~~ — **decided: REST, hand-rolled cache, no TanStack Query.** See §11.
2. **Auth** — Auth0 SPA SDK with redirect or popup; how the token reaches the API client.
3. **How much visual continuity** with mobile is wanted: the purple full-bleed header is the
   app's signature but reads as phone-like on a 1440px screen. Recommendation: keep the
   colour and the pill vocabulary, drop the full-bleed banner in favour of a summary card row.
4. **Whether the web app fixes §9 gaps** or ships at feature parity first. Recommendation:
   fix gaps 1, 9 and 11 in the web app — the API already supports all three (§11.3), they are
   cheap on desktop, and they are exactly what a larger screen is for.

---

## 11. The REST data layer

**Decision: the web app talks to `api/v2` over plain `fetch`. No PowerSync web SDK, no
TanStack Query.** This section is the design that makes that work without the app degrading
into scattered `useEffect` fetches.

### 11.1 What this buys and what it costs

**Buys:** one dependency-free data path, no SQLite-in-the-browser, no sync engine to reason
about, no bundle cost, and a backend that already computes everything the screens need
(§11.3). Debugging is a network tab.

**Costs — each needs a deliberate answer, not a shrug:**

| Lost | Consequence | Answer |
|---|---|---|
| Live local queries | Lists no longer update themselves when data changes elsewhere | A subscribable store per resource; commands invalidate it (§11.2) |
| Optimistic writes | Save is no longer instant; mobile has **no** pending state because it never needed one | Explicit pending state on every submit: disable the action, show progress in the button |
| Offline | The app stops working without a network | Accept it. Say so in the error copy — *"Cannot reach the server"* already exists in the locale files |
| Free cross-screen consistency | Editing an account must refresh the dashboard, the carousel and the detail page | Invalidation groups (§11.2), not per-component refetch |

### 11.2 Architecture — keep the mobile contract, swap the implementation

The mobile hook contract is already transport-agnostic, so **presentation hooks port
unchanged**. Only the repository implementation differs:

```
mobile:  useAccounts → WatchAccountsUseCase → AccountsRepository → AccountsRepositoryInPowersync
web:     useAccounts → WatchAccountsUseCase → AccountsRepository → AccountsRepositoryInApi
```

Four small pieces, all hand-written, roughly 200 lines total:

1. **`ApiClient`** — one `fetch` wrapper. Injects the bearer token, sets `Accept`/
   `Content-Type`, maps HTTP status to `Result.failure(errors)`, parses RFC 7807 problem
   details, and reads `X-Total-Count` for paged lists. Every non-2xx becomes a `Failure`;
   nothing throws past this boundary.

2. **`ResourceStore<T>`** — a tiny observable cache, one instance per resource
   (`accounts`, `transactions`, `cashflows`, `categories`, `tags`, `summary`, …):

   ```ts
   type ResourceStore<T> = {
     subscribe: (key: string, onChange: (result: AsyncResult<T>) => void) => () => void;
     invalidate: (key?: string) => void;   // refetch anything currently subscribed
   };
   ```

   First subscriber triggers the fetch; later subscribers get the cached value; the last
   unsubscribe may keep it briefly. This is what replaces the PowerSync watch, and it is why
   `WatchAccountsUseCase(repo).query(onChange) → unsubscribe` keeps working verbatim.
   Read it from components through `useSyncExternalStore`.

3. **Repositories** — `*-repository-in-api.ts`, one per aggregate, implementing the **same
   interface** as the PowerSync versions. Queries go through the store; commands `POST` and
   then declare what they invalidated.

4. **Invalidation groups** — a command names the resources it dirties, in one place:

   | Command | Invalidates |
   |---|---|
   | make-purchase / transfer / modify / delete transaction | `transactions`, `accounts`, `summary`, `statistics`, `tags` |
   | pay-cashflow | `transactions`, `accounts`, `cashflows`, `upcoming`, `summary` |
   | open / modify / close / favorite account | `accounts` |
   | modify / cancel cashflow | `cashflows`, `upcoming`, `accounts`, `summary` |
   | store-item (settings) | `settings`, `summary`, `statistics`, `upcoming` |

   Nothing else may call `invalidate`. That single table is the whole cache-coherence policy,
   and it is the thing TanStack Query would otherwise be providing.

**Rules to hold the line:**

- Components never call `ApiClient`; they call hooks. Hooks call use cases. Use cases call
  repositories. Same layering as mobile, enforced by the same ESLint import rules.
- Every read returns `AsyncResult<T>` so the three-way state rule of §6.3 survives intact.
- No polling. If a screen needs fresher data, it invalidates on focus/route entry — one
  documented hook, not ad-hoc timers.
- Requests carry an `AbortSignal`; unsubscribing aborts.

### 11.3 The API already covers most screens

`api/v2` is CQRS-shaped and **the queries already do the work the mobile app does locally in
SQL**, which removes a large chunk of client computation:

- `GET api/v2/accounts` returns, per account: `balance`, `updated` (last transaction date),
  `transactionCount`, `upcomingVariation` and the **projected balance** — everything
  `AccountCard` and the account header render, in one call. Mobile fetches each card's
  balance separately; the web app does not have to.
- **Every list endpoint takes `offset`, `limit`, `sort[]`, `filter[]` and returns
  `X-Total-Count`.** Server-side paging, sorting and filtering come free — this is what makes
  the transactions search of gap §9.11 cheap, and it should shape the transactions page from
  day one.
- `GET api/v2/summary/statistics?from&to` takes an explicit range, so **period navigation
  (gap §9.9) is a query parameter**, not new domain code.
- `GET api/v2/periods`, `GET api/v2/cashflows/get-upcoming`, `GET api/v2/tags`,
  `GET api/v2/enumerations/*` cover the remaining reads.
- Commands map one-to-one onto the write paths: `transactions/make-purchase`, `transfer`,
  `modify`, `pay-cashflow`, `DELETE transactions/{id}`, `accounts/open-account`,
  `modify-account`, `close-account`, `favorite-account`, `cashflows/modify`, `cashflows/cancel`,
  `store-items/*`, `users/register`, `users/me`.
- `my-data/export-data` / `import-data` give the web app a real export — better than the
  mobile app's "share the SQLite file" (gap §9.16).

### 11.4 Screen → endpoint map

| Screen | Reads | Writes |
|---|---|---|
| Dashboard | `accounts`, `summary/statistics`, `transactions?limit=3`, `cashflows/get-upcoming` | — |
| Accounts (new) | `accounts` (sort/filter) | — |
| Account detail | `accounts/{id}`, `transactions?filter=accountId&offset&limit` | — |
| Transactions (new) | `transactions` (filter/sort/page) | `DELETE transactions/{id}` |
| Purchase | `accounts`, `categories`, `tags` | `make-purchase` \| `transfer` |
| Edit transaction | `transactions/{id}`, `accounts`, `categories`, `tags` | `transactions/modify` |
| Pay upcoming | `cashflows/get-upcoming` | `pay-cashflow` |
| Add/Edit account | `accounts/{id}`, `enumerations/get-account-types` | `open-account`, `modify-account`, `close-account`, `favorite-account` |
| Manage cashflows | `cashflows` | `cashflows/modify`, `cashflows/cancel` |
| Budget settings | `store-items`, `periods` | `store-items/modify-store-item` |
| Insights | `summary/statistics?from&to`, `categories/statistics`, `categories`, `tags` | — |
| Settings / profile | `users/me` | — |
| Manage categories | `categories` | **missing — see §11.5** |
| Manage tags | `tags` | **missing — see §11.5** |

### 11.5 Backend work the web app pulls in

Three things the mobile app does **only** because PowerSync uploads raw table writes through
`POST api/v2/sync/powersync`, which applies generic `PUT`/`PATCH`/`DELETE` per table. A REST
client has no equivalent, so each needs new `api/v2` endpoints.

**These are built just-in-time, not up front** — a backend slice ships with the web screen
that needs it, in the same increment. Nothing here blocks starting the web app; the read
paths of §11.3 and every transaction, account and cashflow write path are already served.
Listed so the cost is visible when those screens come up in the build order.

1. **Category writes.** There is no create, modify or deactivate command for categories —
   only `GET api/v2/categories` and `GET api/v2/categories/statistics`. This blocks
   **Add Category, Edit Category, delete from Manage Categories, and the onboarding
   categories step**. Needed: `POST categories/create`, `POST categories/modify`,
   `POST categories/deactivate` (soft — never hard-delete, §1).
2. **Tag rename and delete.** Only `GET api/v2/tags` exists. Renaming a tag rewrites it
   across every transaction and cashflow; deleting removes it from both. This is domain
   logic that belongs on the server anyway, not a loop of client-side updates. Needed:
   `POST tags/rename`, `POST tags/delete`.
3. **Insights breakdowns.** `categories/statistics` returns per-category yearly/monthly
   totals — not the mobile model of *spent vs budget vs average-per-period*, and there is no
   tag or category×tag breakdown at all. Two options: add
   `GET api/v2/statistics/category-spending?from&to` (plus tag and combined variants), or
   compute it client-side from `transactions` + `categories` for the period. **Server-side is
   the right call** — the averaging logic already exists in the mobile repository and would
   otherwise be reimplemented a third time.

Sequencing note: **1 is the only one with a dependency outside its own screen** — the
onboarding categories step also needs it, so it lands whenever onboarding or Manage
Categories is built, whichever comes first. 2 and 3 are self-contained and can wait for
their screens. Each follows the backend's normal TDD cycle (`backend/CLAUDE.md`); the
business rules they must honour are in `docs/business-rules/category.md` and the tag rules
in `docs/business-rules/index.md`.

### 11.6 What is deliberately not being rebuilt

- **Offline mode.** No local mirror, no queue, no conflict resolution.
- **Optimistic full-swipe delete** (mobile cashflows, §6.4) — the web version is an explicit
  action with an Undo snackbar, backed by a real request.
- **The DB-share diagnostic screen.** Replace it with `my-data/export-data`; the
  Synchronization screen has no meaning without a sync engine and should not be ported.
  What is worth keeping from it is a small connection/last-updated indicator in Settings.
