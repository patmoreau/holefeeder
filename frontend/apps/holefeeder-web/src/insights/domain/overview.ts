import type { DateOnly } from '@holefeeder/shared/core';

export const NetOutcomes = {
  none: 'none',
  saved: 'saved',
  overspent: 'overspent',
  brokeEven: 'brokeEven',
} as const;

export type NetOutcome = (typeof NetOutcomes)[keyof typeof NetOutcomes];

export const RangeUnits = {
  day: 'day',
  week: 'week',
  month: 'month',
  year: 'year',
} as const;

export type RangeUnit = (typeof RangeUnits)[keyof typeof RangeUnits];

export type Period = { start: DateOnly; end: DateOnly };

export type PeriodFlow = { period: Period; income: number; spending: number };

export type Overview = {
  period: Period;
  income: number;
  spending: number;
  net: number;
  outcome: NetOutcome;
  savedPercentage: number;
  series: PeriodFlow[];
  totalIncome: number;
  totalSpending: number;
  totalNet: number;
  range: { unit: RangeUnit; amount: number };
};

type Raw = Record<string, unknown>;

const flowOf = (value: Raw): PeriodFlow => ({
  period: value.period as Period,
  income: value.income as number,
  spending: value.spending as number,
});

const valid = (value: Raw): Overview => ({
  period: value.period as Period,
  income: value.income as number,
  spending: value.spending as number,
  net: value.net as number,
  outcome: value.outcome as NetOutcome,
  savedPercentage: value.savedPercentage as number,
  series: (value.series as Raw[]).map(flowOf),
  totalIncome: value.totalIncome as number,
  totalSpending: value.totalSpending as number,
  totalNet: value.totalNet as number,
  range: value.range as Overview['range'],
});

export const Overview = {
  valid: valid,
};
