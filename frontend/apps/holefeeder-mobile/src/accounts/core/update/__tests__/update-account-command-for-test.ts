import { AccountTypes, Favorite, Inactive } from '@holefeeder/shared/core';
import { aPastDate, aVariation, anId, aString } from '@holefeeder/shared/testkit';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';

const defaultCommand = (): UpdateAccountCommand => ({
  id: anId(),
  type: AccountTypes.checking,
  name: aString(),
  openBalance: aVariation(),
  openDate: aPastDate(),
  description: aString(),
  favorite: false as Favorite,
  inactive: false as Inactive,
});

export const anUpdateAccountCommand = (overrides?: Partial<UpdateAccountCommand>): UpdateAccountCommand => ({
  ...defaultCommand(),
  ...overrides,
});
