import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';

export type AccountVariation = {
  accountId: Id;
  lastTransactionDate: DateOnly;
  expenses: Money;
  gains: Money;
};

const valid = (value: Record<string, unknown>): AccountVariation => ({
  accountId: Id.valid(value.accountId),
  lastTransactionDate: DateOnly.valid(value.lastTransactionDate),
  expenses: Money.valid(value.expenses),
  gains: Money.valid(value.gains),
});

export const AccountVariation = {
  valid: valid,
};
