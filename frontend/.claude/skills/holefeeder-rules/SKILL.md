# .claude/skills/holefeeder-rules/SKILL.md
---
description: >
  Business rules for Holefeeder — accounts, transactions, categories,
  cashflow, store items. Load when writing tests, reviewing logic, or
  implementing features that touch financial data.
---

Business rules are documented in docs/business-rules/ (canonical, repo root —
shared by the .NET backend and the JS/TS frontend).
Start with the index, then read only the files related to your task:

- Index: docs/business-rules/index.md
- Accounts: docs/business-rules/account.md
- Transactions: docs/business-rules/transaction.md
- Categories: docs/business-rules/category.md
- Cashflow: docs/business-rules/cashflow.md
- Store items: docs/business-rules/store-item.md

When writing tests, assert all invariants listed in the relevant file:
1. Focus on happy path scenarios first.
2. Address edge cases.
3. Implement error handling.

When implementing features, follow the same order to ensure all rules are upheld.