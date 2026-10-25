# Spending advisor

AI-written monthly review of where the money went and where the next period's
spending can come down, shown on mobile Insights and on the web.

**Status (2026-10-25):** draft, not started. Open questions below need answers
before step 1.

## Principle

The backend computes every number; the model only explains them. An LLM never
sums transactions, never sees raw transactions, and never produces a figure the
UI shows as fact. Savings estimates come from the facts, not from the model.

This keeps the arithmetic testable with plain TDD, keeps the data sent to the
model small and anonymous, and leaves the feature useful (rule-based findings)
when the model is unavailable or the user has not opted in.

## Shape

Three phases, each shippable on its own:

1. **Facts and rule-based findings** — no AI. Spending facts endpoint, recurring
   charge detection, findings computed by rules. Already worth shipping.
2. **AI review** — the backend sends the facts to Claude, gets back a structured
   review, validates it and stores it per user per period.
3. **Follow-up questions** — conversational Q&A where the model calls read-only
   tools (existing query endpoints). Separate plan once phase 2 is in use.

Everything ships behind a flag (`FeatureFlags.Temporary.SpendingAdvisor`) until
phase 2 is reviewed on the phone, so `main` stays deployable throughout.

## Decisions proposed

- **Where it runs:** backend only. The API key never reaches the mobile or web
  client. Mobile insights stay local (PowerSync); the advisor card is the one
  Insights piece that needs the network, and it hides when offline.
- **Period:** the user's budget period (anchor, interval type, frequency), not a
  calendar month, so it lines up with the rest of Insights. The review covers the
  last **complete** period, compared with the average of previous periods.
- **Scope of spending:** non-system expense categories only, same as the
  category and tag spending insights (transfers excluded).
- **Data sent to the model:** category names, tag names, period totals, averages,
  deltas, recurring charges (description, amount, interval) and income vs
  expenses. No account names or numbers, no transaction ids, no user identity.
  Free-text descriptions only for detected recurring charges.
- **Model call:** plain `HttpClient` against the Messages API with a JSON schema
  for structured output. No SDK, so no new library to approve. Model id in
  configuration.
- **Generation:** a Hangfire job (Hangfire is already in Infrastructure) runs when
  a period closes, for opted-in users, plus an on-demand "Refresh" limited to once
  a day per user.
- **Consent:** off by default. A settings toggle with a one-paragraph explanation
  of what is sent and to whom. Turning it off deletes stored reviews.
- **Wording:** every review carries "Informational only, not financial advice".

## Open questions

1. **"Computed, never stored."** `statistics.md` says statistics are never
   stored. A generated review is not an aggregate, but it is derived data that
   would be persisted. Options: (a) add an `advisor_reviews` table and state in
   the rules that reviews are generated content, not statistics; (b) never store
   them and generate on each request (cost, latency, a different answer every
   time). Proposal: (a).
2. **Provider and data location.** Is the Anthropic API acceptable for user
   financial aggregates, and under which retention terms? Does the privacy policy
   need an update before the beta?
3. **Web placement.** Dashboard card, or its own page? The web app is a thin MUI
   client, so either way it only renders what the endpoint returns.
4. **Free tier / cost cap.** Budget per user per month, and what happens when it
   is reached (keep the last review, show rule-based findings only).

## Steps

Each step ends with tests green, the Maestro tags run when the mobile app is
touched, and a deploy to the phone before the next step starts. Structural
commits come first inside a step when needed.

### Phase 1 — facts, no AI

#### 1. Spending facts business rules

- New section in `docs/business-rules/statistics.md`: **Spending Facts** — per
  category and per tag for the reviewed period: spent, average of previous
  periods, delta in cents and percent, share of total spending. Same averaging
  rules as category spending (leading empty periods excluded).
- Income vs expenses for the period, net and savings rate (reuse the Net This
  Period definitions).
- No code. Reviewed before step 2.

#### 2. Spending facts endpoint

- `GET api/v2/insights/spending-facts?date={date}` — facts for the complete
  period before the one containing `date`. Built with
  `/backend:add-api-endpoint` as a Carter slice under
  `Features/Statistics/Queries`.
- Tests: period resolution from user settings, empty history, a category with no
  spend this period but a history, system categories excluded, user scoping.

#### 3. Recurring charge detection

- Business rule (`statistics.md`, **Recurring Charges**): a group of expense
  transactions with the same normalised description and category, at least 3
  occurrences, amounts within ±10% of their median, spaced at a regular interval
  (weekly, monthly or yearly within a tolerance of a few days). Declared
  cashflows are reported as recurring directly and are not detected twice.
- Result per charge: description, category, typical amount, interval, monthly
  equivalent, last date, source (`cashflow` or `detected`).
- Pure domain function, unit-tested on hand-built series: irregular gaps, price
  change, skipped month, duplicates of a declared cashflow, yearly charges.
- Added to the facts response.

#### 4. Rule-based findings

- Business rule (**Spending Findings**), each with an estimated monthly saving
  computed from the facts:
  - category above its average by more than 20% and $25 — saving = excess over
    the average;
  - recurring charges per category (e.g. several streaming services) — saving =
    the smallest charges;
  - recurring charge whose amount went up — saving = the increase;
  - expenses above income — no saving, flagged.
- Thresholds in one place, unit-tested at each boundary.
- Added to the facts response, ordered by estimated saving.

#### 5. Advisor card (rule-based) — mobile and web, flagged

- Mobile: a card at the bottom of Insights, "Where to save", listing the top
  three findings with their savings; tapping one opens the matching Spending
  category page. Hidden when offline, empty, or the flag is off.
- Web: same card, rendering the endpoint as-is (no logic in the client).
  Playwright test against the API.
- Maestro: new `advisor` tag.

On the phone: real findings for the last period.

### Phase 2 — AI review

#### 6. Consent setting

- `users.advisor_opt_in` (default false) with its business rule; endpoint to set
  it; settings toggle on mobile and web with the explanation text.
- Turning it off deletes stored reviews (step 8).

#### 7. Model client

- Infrastructure: `IAdvisorModel` with one method taking the facts and returning
  a `SpendingReview`; `HttpClient` implementation for the Messages API, key and
  model id from configuration (secret in the deployment, never in the repo).
- Prompt: system prompt in a versioned resource file; the facts as JSON; a JSON
  schema for the output:
  `{ summary, findings[{ title, category, explanation }],
  recommendations[{ title, findingRef, action }] }`.
- Recommendations reference findings by id and carry **no amounts**; the UI
  shows the savings computed in step 4 next to them.
- Tests with a stubbed `HttpMessageHandler`: request shape (no account names,
  no ids), schema validation, unknown `findingRef` rejected, timeouts and 5xx
  map to a "review unavailable" result, never an exception to the client.

#### 8. Review storage and generation

- `advisor_reviews` table (user, period start, prompt version, model id, JSON
  review, created at), one row per user per period, depending on open question 1.
- Hangfire job when a period closes for opted-in users; on-demand refresh at most
  once a day per user (429 otherwise).
- `GET api/v2/insights/spending-review?date={date}` returns the stored review,
  or 404 so the client falls back to the rule-based card.
- Tests: job idempotent per period, rate limit, opt-out deletes, user scoping.

#### 9. AI review on the card

- The card shows the summary paragraph above the findings, recommendations under
  each finding, the disclaimer and the generation date. Falls back to step 5's
  content without a review.
- Manual review of the advice on real data over a few periods before the flag
  is removed.

#### 10. Remove the flag

- Remove `SpendingAdvisor` from `FeatureFlags.Temporary` and the dead branches.
- Update `docs/business-rules/index.md` status table.

### Phase 3 — follow-up questions

Separate plan. Outline: a chat page that sends the question and the stored facts
to the model with read-only tools (transactions by category and period, category
statistics, cashflows), executed server-side under the caller's user scope, with
a per-user daily cap.

## Cost estimate

One review is about 3–5k input tokens and 1k output tokens per user per period,
a few cents at most. Prompt caching on the system prompt reduces it further.
The daily refresh cap bounds the worst case.
