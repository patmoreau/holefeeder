import { AccountType } from '@/accounts/core/account-type';
import { Id } from '@/shared/core/id';

export type AccountSummary = {
  id: Id;
  name: string;
  type: AccountType;
};

const valid = (value: Record<string, unknown>): AccountSummary => ({
  id: Id.valid(value.id),
  name: value.name as string,
  type: AccountType.valid(value.type),
});

export const AccountSummary = {
  valid: valid,
};
