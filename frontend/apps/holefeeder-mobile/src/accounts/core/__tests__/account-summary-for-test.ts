import { anAccountType } from '@/shared/__tests__/enum-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
import { AccountSummary } from '../account-summary';

export const anAccountSummary = (overrides?: Partial<AccountSummary>): AccountSummary => ({
  id: anId(),
  name: aString(),
  type: anAccountType(),
  ...overrides,
});
