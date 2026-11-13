import { aRecentDate, anAmount, anId } from '@holefeeder/shared/testkit';
import { AccountVariation } from '@/flows/core/flows/account-variation';

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
