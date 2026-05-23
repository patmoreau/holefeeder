# .claude/skills/holefeeder-rules/SKILL.md
---
description: >
  Business rules for Holefeeder — accounts, transactions, budgets, cashflow.
  Load when writing tests, reviewing logic, or implementing features that
  touch financial data.
---

Business rules are documented in docs/business-rules/.
Read only the files directly related to your task before proceeding:

- Accounts: docs/business-rules/accounts.md
- Transactions: docs/business-rules/transactions.md
- Budgets: docs/business-rules/budgets.md
- Cashflow: docs/business-rules/cashflow.md

When writing tests, assert all invariants listed in the relevant file:
1. Focus on happy path scenarios first.
2. Address edge cases.
3. Implement error handling.

When implementing features, follow the same order to ensure all rules are upheld.