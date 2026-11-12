---
path: docs/business-rules/tag.md
domain: tag
last-reviewed: 2026-11
---

# Tag Rules

## Concepts

A tag is a free-form label on transactions and cashflows. Tags are not stored on their
own: each transaction and cashflow keeps its tags as a comma-separated list, so a tag
exists while at least one transaction or cashflow carries it. Tags are normalized to
lowercase when a transaction or cashflow is saved (see [Transaction Rules](transaction.md)
and [Cashflow Rules](cashflow.md#tags)).

---

## Listing Tags to Manage

Lists every tag the user has, with how much each one is used.

- A tag is listed when it is on one of the user's transactions or active cashflows
- Tags on inactive (cancelled) cashflows only are not listed
- Each tag carries the number of transactions and the number of active cashflows using it
- Blank tags are ignored
- Tags are sorted by name
- Only the user's own transactions and cashflows are considered

---

## Renaming a Tag

Replaces a tag with a new name on every transaction and cashflow of the user.

- The tag to rename must not be empty; it is matched exactly, after trimming, so `car`
  does not match `carwash` and `Car` does not match `car`
- The new name is trimmed and lowercased; it must not be empty or contain a comma
- Applies to transactions and to all cashflows, cancelled ones included
- The renamed tag keeps its position in each list
- When an item already carries the new name, the two merge into one, at the position of
  the first one
- Renaming a tag to its own name changes nothing
- Only the user's own transactions and cashflows change
