import { DateOnly, Id, today } from '@holefeeder/shared/core';
import { AccountType, AccountTypes } from '@/accounts/core/account-type';

export type EditAccountFormData = {
  // null means the account does not exist yet, the way the category form says it.
  id: Id | null;
  name: string;
  type: AccountType;
  openBalance: number;
  openDate: DateOnly;
  description: string;
  favorite: boolean;
  inactive: boolean;
};

const forNewAccount = (overrides?: Partial<EditAccountFormData>): EditAccountFormData => ({
  id: null,
  name: '',
  type: AccountTypes.checking,
  openBalance: 0,
  openDate: DateOnly.valid(today()),
  description: '',
  favorite: false,
  inactive: false,
  ...overrides,
});

export const EditAccountFormData = {
  forNewAccount: forNewAccount,
};
