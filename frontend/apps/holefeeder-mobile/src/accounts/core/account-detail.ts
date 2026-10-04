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

const upcomingChange = (detail: AccountDetail): Variation => Variation.multiply(detail.upcomingVariation, AccountType.multiplier[detail.type]);

const isUpcomingFavourable = (detail: AccountDetail): boolean => detail.upcomingVariation >= 0;

const ownedBalance = (detail: AccountDetail): Variation => Variation.multiply(detail.balance, AccountType.multiplier[detail.type]);

export type BalanceCaption = 'availableBalance' | 'amountDue' | 'creditBalance';

const balanceCaption = (detail: AccountDetail): BalanceCaption => {
  const isAsset = AccountType.multiplier[detail.type] > 0;
  const owned = Math.round(ownedBalance(detail) * 100);
  if (owned < 0) return 'amountDue';
  if (owned > 0) return isAsset ? 'availableBalance' : 'creditBalance';
  return isAsset ? 'availableBalance' : 'amountDue';
};

const ownedProjectedBalance = (detail: AccountDetail): Variation =>
  Variation.multiply(detail.projectedBalance, AccountType.multiplier[detail.type]);

export const AccountDetail = {
  valid: valid,
  upcomingChange: upcomingChange,
  isUpcomingFavourable: isUpcomingFavourable,
  ownedBalance: ownedBalance,
  ownedProjectedBalance: ownedProjectedBalance,
  balanceCaption: balanceCaption,
};
