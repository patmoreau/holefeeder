import { DateOnly, Id, Variation } from '@holefeeder/shared/core';
import { AccountType } from '@/accounts/core/account-type';

export type AccountDetail = {
  id: Id;
  name: string;
  type: AccountType;
  balance: Variation;
  lastTransactionDate: DateOnly;
  projectedBalance: Variation;
  upcomingVariation: Variation;
};

const valid = (value: Record<string, unknown>): AccountDetail => ({
  id: Id.valid(value.id),
  name: value.name as string,
  type: AccountType.valid(value.type),
  balance: Variation.valid(value.balance),
  lastTransactionDate: DateOnly.valid(value.lastTransactionDate),
  projectedBalance: Variation.valid(value.projectedBalance),
  upcomingVariation: Variation.valid(value.upcomingVariation),
});

export const AccountDetail = {
  valid: valid,
};
