import { Favorite } from '@holefeeder/shared/core';
import { AccountTypes } from '@/accounts/core/account-type';
import { CreateAccountCommand } from '@/accounts/core/create/create-account-command';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aVariation } from '@/shared/__tests__/number-for-test';
import { aString } from '@/shared/__tests__/string-for-test';

const defaultCommand = (): CreateAccountCommand => ({
  type: AccountTypes.checking,
  name: aString(),
  openBalance: aVariation(),
  openDate: aPastDate(),
  description: aString(),
  favorite: false as Favorite,
});

export const aCreateAccountCommand = (overrides?: Partial<CreateAccountCommand>): CreateAccountCommand => ({
  ...defaultCommand(),
  ...overrides,
});
