---
path: docs/business-rules/transaction.md
domain: transaction
last-reviewed: 2026-11
---

# Transaction Rules

## Concepts

A transaction is a record of a financial movement on an account for a specific date and amount. Every transaction must reference an account, a category, and a user-declared date.

A transaction may optionally be linked to a cashflow, recording which scheduled occurrence it covers. See [Cashflow Rules](cashflow.md) for cashflow rules.

---

## Displaying Transactions

The description is optional, so a transaction is titled by what the user wrote, falling back
to its category:

- **Title** — the description; when it is empty or only spaces, the category name
- **Category line** — the category name is shown under the title only when the title is a
  real description, never twice
- **Icon** — the category's icon in the category's colour, so rows of different categories
  can be told apart at a glance

The description itself is stored as entered: an empty description stays empty and is never
replaced by the category name in the data. The same rules apply to upcoming cashflow
occurrences.

---

## Make Purchase

Records a one-off purchase or expense on an account.

**Happy path:**

- Date, account, and category are provided
- Account and category exist and belong to the current user
- A transaction is created; tags are normalized (lowercase, deduplicated, blanks removed)
- A recurring cashflow schedule can optionally be created at the same time and linked to the transaction (see [Cashflow Rules](cashflow.md))

**Errors:**

| Condition                                          | Error                |
|----------------------------------------------------|----------------------|
| Date is missing                                    | rejected             |
| AccountId is empty                                 | rejected             |
| CategoryId is empty                                | rejected             |
| Account does not exist or belongs to another user  | `Account not found`  |
| Category does not exist or belongs to another user | `Category not found` |
| Amount is negative or has more than two decimals   | rejected             |
| Repeat frequency is 0 or less                      | rejected             |
| Repeat interval type is unknown                    | rejected             |

**Repeating a purchase** creates its cashflow in the same step: same account, category,
amount, description and tags, with the chosen start date, interval type and frequency,
and no limit on occurrences. The purchase is recorded as the payment of that cashflow's
occurrence on the purchase date. Nothing is saved when any part fails.

---

## Modify Transaction

Updates the fields of an existing transaction.

**Happy path:**

- Transaction ID, date, account, and category are all provided
- Transaction, account, and category all exist and belong to the current user
- Date, amount, description, account, category, and tags can all be changed

**Errors:**

| Condition                                             | Error                   |
|-------------------------------------------------------|-------------------------|
| Transaction ID is empty                               | rejected                |
| Date is missing                                       | rejected                |
| AccountId is empty                                    | rejected                |
| CategoryId is empty                                   | rejected                |
| Account does not exist or belongs to another user     | `Account not found`     |
| Category does not exist or belongs to another user    | `Category not found`    |
| Transaction does not exist or belongs to another user | `Transaction not found` |

---

## Delete Transaction

Permanently removes a transaction.

**Happy path:**

- Transaction ID is provided
- Transaction exists and belongs to the current user
- Transaction is permanently removed

**Errors:**

| Condition                                             | Error                   |
|-------------------------------------------------------|-------------------------|
| Transaction ID is empty                               | rejected                |
| Transaction does not exist or belongs to another user | `Transaction not found` |

---

## Transfer

Moves money between two accounts. Creates two transactions atomically.

**Happy path:**

- Date, source account, destination account, and amount are all provided
- Both accounts are active and belong to the current user
- The user has system categories named "Transfer Out" and "Transfer In"
- A debit transaction is created on the source account (categorized as "Transfer Out")
- A credit transaction is created on the destination account (categorized as "Transfer In")
- If no description is provided, it defaults to `"Transfer from '{source}' to '{destination}'"`

**Errors:**

| Condition                                                                   | Error                |
|-----------------------------------------------------------------------------|----------------------|
| Date is missing                                                             | rejected             |
| FromAccountId is empty                                                      | rejected             |
| ToAccountId is empty                                                        | rejected             |
| Source account does not exist, is inactive, or belongs to another user      | `Account not found`  |
| Destination account does not exist, is inactive, or belongs to another user | `Account not found`  |
| Source and destination are the same account                                 | `Same account`       |
| "Transfer Out" category not found for the user                              | `Category not found` |
| "Transfer In" category not found for the user                               | `Category not found` |

---

## New Transaction Form

The form for a new expense, income or transfer starts from these defaults:

- **Type** — expense
- **Date** — today
- **Amount** — zero
- **Account** — the first open account, favourites first and then by name; a transfer goes
  to the next account in that order, when there is one
- **Category** — the first active category of the type picked (expense or income), never a
  system category, favourites first and then by name; switching the type switches the
  category to the first of the new type
- **Repeat** — off; when turned on, it starts on the transaction date, every 1 month

The form cannot be used without an open account or a non-system expense category; it points
the user to create one instead.

**Tag suggestions** list every tag the user has used, most relevant first: tags used with the
chosen category come first, by how often they were used with it, then the others by how
often they were used overall, then alphabetically. Tags already picked are not suggested.

---

## Tags

- Tags are normalized to lowercase on save
- Empty or whitespace-only tags are discarded
- Duplicate tags are removed
- An empty tag list is valid and clears all existing tags

---

## Invariants

- A transfer always creates exactly two transactions (debit + credit); neither is persisted without the other
- A transaction linked to a cashflow must carry both a cashflow reference and the scheduled cashflow date — the two are set together or not at all
- All operations are scoped to the owning user — cross-user access is rejected at every step
