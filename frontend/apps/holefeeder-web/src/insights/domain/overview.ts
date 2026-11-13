import type { DateOnly, NetBalanceKind, PeriodSpan } from '@holefeeder/shared/core';

export type Period = { start: DateOnly; end: DateOnly };

export type PeriodFlow = { period: Period; income: number; spending: number; net: number };

export type RangeOption = { periods: number; range: PeriodSpan };

export type Overview = {
  period: Period;
  income: number;
  spending: number;
  net: number;
  outcome: NetBalanceKind;
  savedPercentage: number;
  series: PeriodFlow[];
  totalIncome: number;
  totalSpending: number;
  totalNet: number;
  range: PeriodSpan;
  ranges: RangeOption[];
};

type Raw = Record<string, unknown>;

const flowOf = (value: Raw): PeriodFlow => ({
  period: value.period as Period,
  income: value.income as number,
  spending: value.spending as number,
  net: value.net as number,
});

const valid = (value: Raw): Overview => ({
  period: value.period as Period,
  income: value.income as number,
  spending: value.spending as number,
  net: value.net as number,
  outcome: value.outcome as NetBalanceKind,
  savedPercentage: value.savedPercentage as number,
  series: (value.series as Raw[]).map(flowOf),
  totalIncome: value.totalIncome as number,
  totalSpending: value.totalSpending as number,
  totalNet: value.totalNet as number,
  range: value.range as PeriodSpan,
  ranges: value.ranges as RangeOption[],
});

export const Overview = {
  valid: valid,
};
