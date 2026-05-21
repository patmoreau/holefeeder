# Business Rules Index

## Domains

| Domain      | File                               | Key Invariants                                                                     |
|-------------|------------------------------------|------------------------------------------------------------------------------------|
| Accounts    | [accounts.md](accounts.md)         | Balance is always computed; closed accounts cannot be reopened or closed again     |
| Cashflows   | [cashflows.md](cashflows.md)       | Only active cashflows generate upcoming dates; cancelled cashflows cannot be reactivated |
| Categories  | [categories.md](categories.md)     | Managed via import; system categories required for transfers                       |
| Store Items | [store-items.md](store-items.md)   | Code is immutable after creation; codes are unique per user                        |
| Transactions | [transactions.md](transactions.md) | Transfers always create exactly two transactions; atomically or not at all        |

---

## Cross-Domain Rules

### Money
- All monetary amounts must be **≥ 0** — negative values are rejected at the value-object level, before any domain logic runs
- Zero is a valid amount (e.g., opening balance of a new account)
- Amounts are stored as `decimal`, not integers or cents

### Category Color
- Category colors must be a valid **HTML color string** (e.g., `#FF5733`)
- An invalid color string is rejected when the category is created or imported

### Balance Calculation
Account balance is always computed, never stored:

> **Balance = Opening balance + Σ (amount × account type sign × category type sign)**

- Debit account types (Checking, Investment, Savings) use sign **+1**
- Credit account types (CreditCard, CreditLine, Loan, Mortgage) use sign **−1**
- Gain categories use sign **+1**; Expense categories use sign **−1**

### Tags
Tags behave the same way on both transactions and cashflows:
- Normalized to **lowercase** on save
- **Blank** and whitespace-only tags are discarded
- **Duplicates** are removed
- An empty tag list is valid and clears all existing tags
- Tags on a cashflow are **inherited** by transactions created when paying an occurrence

### User Scoping
Every entity (account, cashflow, category, store item, transaction) is scoped to the owning user. All queries and mutations filter by the authenticated user's ID — cross-user access is rejected at every step.

### Inactive Pattern
Soft inactivation (not hard deletion) is used for:
- **Accounts** — closed via Close Account; excluded from active workflows
- **Cashflows** — cancelled via Cancel Cashflow; produce no upcoming dates

Transactions are **hard deleted** — they are permanently removed when deleted.

---

## Status

| File            | Last reviewed | Matches code? |
|-----------------|---------------|---------------|
| accounts.md     | 2026-05       | ✅             |
| cashflows.md    | 2026-05       | ✅             |
| categories.md   | 2026-05       | ✅             |
| store-items.md  | 2026-05       | ✅             |
| transactions.md | 2026-05       | ✅             |
