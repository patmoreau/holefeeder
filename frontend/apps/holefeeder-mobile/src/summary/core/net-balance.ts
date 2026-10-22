import { ComputedSummary } from '@/summary/core/watch-summary/watch-summary-use-case';

export const NetBalanceKinds = {
  saved: 'saved',
  overspent: 'overspent',
  brokeEven: 'brokeEven',
  none: 'none',
} as const;

export type NetBalanceKind = (typeof NetBalanceKinds)[keyof typeof NetBalanceKinds];

export type NetBalanceResult =
  | { kind: typeof NetBalanceKinds.saved; amount: number; percentage: number }
  | { kind: typeof NetBalanceKinds.overspent; amount: number }
  | { kind: typeof NetBalanceKinds.brokeEven }
  | { kind: typeof NetBalanceKinds.none };

const toCents = (amount: number) => Math.round(amount * 100);

const between = (income: number, spending: number): NetBalanceResult => {
  const incomeCents = toCents(income);
  const spendingCents = toCents(spending);
  const netCents = incomeCents - spendingCents;

  if (incomeCents === 0 && spendingCents === 0) return { kind: NetBalanceKinds.none };
  if (netCents === 0) return { kind: NetBalanceKinds.brokeEven };
  if (netCents < 0) return { kind: NetBalanceKinds.overspent, amount: -netCents / 100 };

  return { kind: NetBalanceKinds.saved, amount: netCents / 100, percentage: Math.round((netCents / incomeCents) * 100) };
};

const of = (summary: ComputedSummary): NetBalanceResult => between(summary.totalIncome, summary.currentSpending);

export const NetBalance = {
  of: of,
  between: between,
};
