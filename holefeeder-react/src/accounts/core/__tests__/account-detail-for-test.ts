import { Account } from '@/accounts/core/account';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aVariation } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
import { AccountDetail } from '../account-detail';

const defaultAccountDetail = (): AccountDetail => ({
  id: anId(),
  name: aString(),
  balance: aVariation(),
  lastTransactionDate: aPastDate(),
  projectedBalance: aVariation(),
  upcomingVariation: aVariation(),
});

export const anAccountDetail = (overrides?: Partial<Account>): AccountDetail => ({
  ...defaultAccountDetail(),
  ...overrides,
});
