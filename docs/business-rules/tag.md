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
