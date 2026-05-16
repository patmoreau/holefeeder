import { AccountType } from '@/accounts/core/account-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';

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
