import { DateOnly, Id } from '@holefeeder/shared/core';
import { AccountType } from '@/accounts/core/account-type';

export type EditAccountFormData = {
  id: Id;
  name: string;
  type: AccountType;
  openBalance: number;
  openDate: DateOnly;
  description: string;
  favorite: boolean;
  inactive: boolean;
};
