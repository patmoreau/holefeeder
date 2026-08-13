import { DateOnly, Id } from '@holefeeder/shared/core';
import { AccountType } from '@/accounts/core/account-type';

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
