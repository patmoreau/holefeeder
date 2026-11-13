import { AccountTypes, Favorite } from '@holefeeder/shared/core';
import { aPastDate, aVariation, aString } from '@holefeeder/shared/testkit';
import { CreateAccountCommand } from '@/accounts/core/create/create-account-command';

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
