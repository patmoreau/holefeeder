import { tk } from '@holefeeder/shared/core';

export const AccountTypes = {
  checking: 'Checking',
  savings: 'Savings',
  investment: 'Investment',
  creditCard: 'CreditCard',
  creditLine: 'CreditLine',
  loan: 'Loan',
  mortgage: 'Mortgage',
} as const;

export type AccountType = (typeof AccountTypes)[keyof typeof AccountTypes];

export const accountTypeKey: Record<AccountType, string> = {
  Checking: tk.accountEdit.accountTypes.checking,
  Savings: tk.accountEdit.accountTypes.savings,
  Investment: tk.accountEdit.accountTypes.investment,
  CreditCard: tk.accountEdit.accountTypes.creditCard,
  CreditLine: tk.accountEdit.accountTypes.creditLine,
  Loan: tk.accountEdit.accountTypes.loan,
  Mortgage: tk.accountEdit.accountTypes.mortgage,
};
