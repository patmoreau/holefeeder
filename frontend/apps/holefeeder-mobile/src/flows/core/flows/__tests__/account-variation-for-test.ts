import { AccountVariation } from '@/flows/core/flows/account-variation';
import { aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId } from '@/shared/__tests__/string-for-test';

const defaultAccountVariation = (): AccountVariation => ({
  accountId: anId(),
  lastTransactionDate: aRecentDate(),
  expenses: anAmount(),
  gains: anAmount(),
});

export const anAccountVariation = (overrides?: Partial<AccountVariation>): AccountVariation => ({
  ...defaultAccountVariation(),
  ...overrides,
});
