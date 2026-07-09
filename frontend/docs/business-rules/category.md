---
path: docs/business-rules/category.md
domain: category
last-reviewed: 2026-05
---

# Category Rules

## Concepts

A category classifies transactions and cashflows. Every transaction must belong to exactly one category. Categories have a type that determines how they affect account balances, a display colour, and
an optional budget amount for planning purposes.

Categories are scoped to the owning user and are managed through data import rather than individual create/modify/delete operations.

---

## Category Types

| Type    | Effect on balance             |
|---------|-------------------------------|
| Expense | Decreases the account balance |
| Gain    | Increases the account balance |

The sign of a category type combines with the sign of the account type to determine the net effect of a transaction on the account balance.

---

## System Categories

Some categories are flagged as system categories. These are created automatically and are required for built-in features:

| Name         | Purpose                                          |
|--------------|--------------------------------------------------|
| Transfer Out | Applied to the source account in a transfer      |
| Transfer In  | Applied to the destination account in a transfer |

The Transfer feature will fail if these categories do not exist for the user (see [Transaction Rules — Transfer](transaction.md#transfer)).

---

## Favourite Categories

A category can be marked as a favourite to make it easier to find in the UI. This flag has no effect on balance calculations or business logic.

---

## Budget Amount

Each category can carry a budget amount used for planning and statistics. A budget amount of 0 means no budget has been set.

---

## Constraints

- Name must be between 1 and 255 characters
- All categories are scoped to the owning user — cross-user access is rejected
