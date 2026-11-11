---
path: docs/business-rules/settings.md
domain: settings
last-reviewed: 2026-11
---

# Settings Rules

## Concepts

A user's settings define their **budget period**: an effective (anchor) date, an interval
type and a frequency. Periods step from the effective date by `frequency` intervals, and
the current period is the one containing today (see [statistics.md](statistics.md)).

Settings are kept in the store item with code `settings`, whose data is
`{"effectiveDate":"YYYY-MM-DD","intervalType":"monthly","frequency":1}` (interval type in
camelCase). The mobile app writes that store item directly; the web app goes through
`GET api/v2/settings` and `POST api/v2/settings/save-settings`.

---

## Get Settings

- Returns the user's effective date, interval type and frequency.
- A user who never saved settings gets the defaults: **monthly, every 1, from today**.
- Stored data that cannot be read is an unexpected error, not a silent default.

---

## Save Settings

**Happy path:**

- Effective date, interval type and frequency are provided and valid
- The `settings` store item is created, or its data replaced when it exists

**Errors:**

| Condition                                                   | Error                             |
|-------------------------------------------------------------|-----------------------------------|
| Effective date missing or not `YYYY-MM-DD`                  | `settings-effective-date-invalid` |
| Interval type missing or not daily, weekly, monthly, yearly or one-time | `interval-type-invalid` |
| Frequency missing, not a whole number, or below 1           | `period-settings-frequency-invalid` |

---

## Invariants

- A user has at most one `settings` store item.
- Frequency is always at least 1.
- Settings are scoped to the owning user.
