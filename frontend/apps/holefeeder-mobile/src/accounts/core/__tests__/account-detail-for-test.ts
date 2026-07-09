import { Account } from '@/accounts/core/account';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { anAccountType } from '@/shared/__tests__/enum-for-test';
import { aVariation } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
import { AccountDetail } from '../account-detail';

const defaultAccountDetail = (): AccountDetail => ({
  id: anId(),
  name: aString(),
  type: anAccountType(),
  balance: aVariation(),
  lastTransactionDate: aPastDate(),
  projectedBalance: aVariation(),
  upcomingVariation: aVariation(),
});

export const anAccountDetail = (overrides?: Partial<Account>): AccountDetail => ({
  ...defaultAccountDetail(),
  ...overrides,
});
