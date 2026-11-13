import { type BalanceCaption, DateOnly } from '@holefeeder/shared/core';
import type { AccountType } from '@/shared/domain/account-type';

export type AccountDetail = {
  id: string;
  name: string;
  type: AccountType;
  description: string;
  openBalance: number;
  openDate: DateOnly;
  favorite: boolean;
  inactive: boolean;
  caption: BalanceCaption;
  balance: number;
  upcoming: number;
  projectedBalance: number;
  updated: DateOnly;
};

const valid = (value: Record<string, unknown>): AccountDetail => ({
  id: value.id as string,
  name: value.name as string,
  type: value.type as AccountType,
  description: (value.description as string | null) ?? '',
  openBalance: value.openBalance as number,
  openDate: DateOnly.valid(value.openDate),
  favorite: value.favorite as boolean,
  inactive: value.inactive as boolean,
  caption: value.caption as BalanceCaption,
  balance: value.balance as number,
  upcoming: value.upcoming as number,
  projectedBalance: value.projectedBalance as number,
  updated: DateOnly.valid(value.updated),
});

export const AccountDetail = { valid: valid };
