# Cashflows

## Concepts

A cashflow is a recurring payment schedule that projects expected transactions at defined intervals. It is either active or inactive (cancelled). When a scheduled payment is made, it is recorded as a
transaction linked back to the cashflow (see [Transactions](transactions.md)).

---

## Interval Types

| Type    | Behaviour                                      |
|---------|------------------------------------------------|
| OneTime | Fires exactly once on the effective date       |
| Daily   | Repeats every N days from the effective date   |
| Weekly  | Repeats every N weeks from the effective date  |
| Monthly | Repeats every N months from the effective date |
| Yearly  | Repeats every N years from the effective date  |

- Frequency (N) must be at least 1
- Recurrence (maximum number of occurrences; 0 = unlimited) must be 0 or greater

---

## Upcoming Occurrences

Given a target end date, the cashflow computes which scheduled dates are still unpaid:

- An inactive cashflow produces no upcoming dates
- If no payments have been recorded: all scheduled dates from the effective date up to the target date are listed
- If payments have been recorded: only scheduled dates after the most-recently paid cashflow date are listed
- A one-time cashflow that has already been paid produces no upcoming dates

---

## Create

A cashflow is created alongside a Make Purchase request (see [Transactions — Make Purchase](transactions.md#make-purchase)). It inherits the transaction's account, category, amount, description, and
tags.

**Errors:**

| Condition                | Error                                      |
|--------------------------|--------------------------------------------|
| EffectiveDate is missing | rejected                                   |
| Frequency is 0 or less   | rejected (`Frequency must be positive`)    |
| Recurrence is negative   | rejected (`Recurrence cannot be negative`) |
| AccountId is empty       | rejected                                   |
| CategoryId is empty      | rejected                                   |

---

## Pay Cashflow

Records a payment against a scheduled cashflow occurrence by creating a linked transaction.

**Happy path:**

- Date, cashflow ID, and cashflow date are all provided
- Cashflow exists and belongs to the current user
- A transaction is created using the cashflow's account, category, description, and tags
- The transaction records both the actual payment date and the scheduled cashflow date it covers

**Errors:**

| Condition                                          | Error                |
|----------------------------------------------------|----------------------|
| Date is missing                                    | rejected             |
| CashflowId is empty                                | rejected             |
| CashflowDate is missing                            | rejected             |
| Cashflow does not exist or belongs to another user | `Cashflow not found` |

---

## Modify Cashflow

Updates the amount, effective date, description, or tags of an existing cashflow.

**Happy path:**

- Cashflow ID and effective date are provided
- Cashflow exists and belongs to the current user
- Amount, effective date, description, and tags can be updated

**Errors:**

| Condition                                          | Error                |
|----------------------------------------------------|----------------------|
| CashflowId is empty                                | rejected             |
| EffectiveDate is missing                           | rejected             |
| Cashflow does not exist or belongs to another user | `Cashflow not found` |

---

## Cancel Cashflow

Marks a cashflow as inactive. It will no longer appear in upcoming schedules.

**Happy path:**

- Cashflow ID is provided
- Cashflow exists, belongs to the current user, and is currently active
- Cashflow is set to inactive

**Errors:**

| Condition                                          | Error                       |
|----------------------------------------------------|-----------------------------|
| CashflowId is empty                                | rejected                    |
| Cashflow does not exist or belongs to another user | `Cashflow not found`        |
| Cashflow is already inactive                       | `Cashflow already inactive` |

---

## Tags

- Tags are normalized to lowercase on save
- Empty or whitespace-only tags are discarded
- Duplicate tags are removed
- An empty tag list is valid and clears all existing tags
- Tags are inherited by transactions created when paying a cashflow occurrence

---

## Invariants

- All operations are scoped to the owning user — cross-user access is rejected at every step
- A cancelled cashflow cannot be reactivated
