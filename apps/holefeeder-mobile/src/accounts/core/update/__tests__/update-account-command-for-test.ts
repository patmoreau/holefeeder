import { AccountTypes } from '@/accounts/core/account-type';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aVariation } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';

const defaultCommand = (): UpdateAccountCommand => ({
  id: anId(),
  type: AccountTypes.checking,
  name: aString(),
  openBalance: aVariation(),
  openDate: aPastDate(),
  description: aString(),
  favorite: false,
  inactive: false,
});

export const anUpdateAccountCommand = (overrides?: Partial<UpdateAccountCommand>): UpdateAccountCommand => ({
  ...defaultCommand(),
  ...overrides,
});
