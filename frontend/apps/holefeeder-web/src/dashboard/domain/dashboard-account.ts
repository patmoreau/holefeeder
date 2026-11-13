import { type BalanceCaption, DateOnly } from '@holefeeder/shared/core';

export type DashboardAccount = {
  id: string;
  name: string;
  caption: BalanceCaption;
  balance: number;
  projectedBalance: number;
  updated: DateOnly;
};

const valid = (value: Record<string, unknown>): DashboardAccount => ({
  id: value.id as string,
  name: value.name as string,
  caption: value.caption as BalanceCaption,
  balance: value.balance as number,
  projectedBalance: value.projectedBalance as number,
  updated: DateOnly.valid(value.updated),
});

const validList = (values: Record<string, unknown>[]): DashboardAccount[] => values.map(valid);

export const DashboardAccount = { valid: valid, validList: validList };
