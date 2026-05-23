---
path: docs/business-rules/account.md
domain: account
last-reviewed: 2026-05
---

# Account Rules

## Concepts

An account tracks financial movements for a user. It has a type, a name, an opening date, an opening balance, and an optional description. Accounts can be marked as a favourite and are either active
or inactive (closed).

The current balance is always computed: opening balance plus the sum of all transaction amounts, each adjusted by the account type's sign and the transaction's category type sign.

---

## Account Types

| Type       | Sign     |
|------------|----------|
| Checking   | Positive |
| Investment | Positive |
| Savings    | Positive |
| CreditCard | Negative |
| CreditLine | Negative |
| Loan       | Negative |
| Mortgage   | Negative |

For positive-sign accounts, gains increase the balance and expenses decrease it. For negative-sign accounts, the effect is reversed.

---

## Open Account

Creates a new account for the current user.

**Happy path:**

- Type, name, and opening date are provided
- The name is unique for this user
- Account is created as active, non-favourite, with the provided opening balance (defaults to 0)

**Errors:**

| Condition                                                | Error                 |
|----------------------------------------------------------|-----------------------|
| Type is missing                                          | rejected              |
| Name is empty, whitespace, or longer than 255 characters | rejected              |
| Opening date is missing                                  | rejected              |
| Name already exists for this user                        | `Name already exists` |

---

## Modify Account

Updates an existing account's name, opening balance, or description.

**Happy path:**

- Account ID and name are provided
- Account exists and belongs to the current user
- Name, opening balance, and description can be changed

**Errors:**

| Condition                                                | Error               |
|----------------------------------------------------------|---------------------|
| Account ID is empty                                      | rejected            |
| Name is empty, whitespace, or longer than 255 characters | rejected            |
| Account does not exist or belongs to another user        | `Account not found` |

---

## Set Favourite

Marks or unmarks an account as a favourite.

**Happy path:**

- Account ID and the desired favourite state are provided
- Account exists and belongs to the current user
- The favourite flag is updated

**Errors:**

| Condition                                         | Error               |
|---------------------------------------------------|---------------------|
| Account ID is empty                               | rejected            |
| Account does not exist or belongs to another user | `Account not found` |

---

## Close Account

Marks an account as inactive. Closed accounts are excluded from active workflows.

**Happy path:**

- Account ID is provided
- Account exists, belongs to the current user, and is currently active
- Account has no active cashflows
- Account is set to inactive

**Errors:**

| Condition                                         | Error                          |
|---------------------------------------------------|--------------------------------|
| Account ID is empty                               | rejected                       |
| Account does not exist or belongs to another user | `Account not found`            |
| Account is already closed                         | `Account already closed`       |
| Account has active cashflows                      | `Account has active cashflows` |

---

## Invariants

- A closed account cannot be closed again
- A closed account cannot be reopened
- An account with active cashflows cannot be closed — cancel or complete all cashflows first
- Account names must be unique per user
- Account type cannot be changed after opening
