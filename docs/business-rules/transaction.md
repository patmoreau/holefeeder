---
path: docs/business-rules/transaction.md
domain: transaction
last-reviewed: 2026-05
---

# Transaction Rules

## Concepts

A transaction is a record of a financial movement on an account for a specific date and amount. Every transaction must reference an account, a category, and a user-declared date.

A transaction may optionally be linked to a cashflow, recording which scheduled occurrence it covers. See [Cashflow Rules](cashflow.md) for cashflow rules.

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
| "Transfer Out" category not found for the user                              | `Category not found` |
| "Transfer In" category not found for the user                               | `Category not found` |

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
