import type { DateOnly } from '@holefeeder/shared/core';

export const BudgetStatuses = {
  none: 'none',
  under: 'under',
  over: 'over',
} as const;

export type BudgetStatus = (typeof BudgetStatuses)[keyof typeof BudgetStatuses];

export type TagAmount = { tag: string; amount: number };

export type CategoryBreakdown = {
  id: string;
  name: string;
  color: string;
  budgetStatus: BudgetStatus;
  budgetPercentage: number;
  amount: number;
  average: number;
  tags: TagAmount[];
};

export type TagBreakdown = { tag: string; amount: number; average: number };

export type Breakdown = {
  period: { start: DateOnly; end: DateOnly };
  current: number;
  average: number;
  variation: number;
  categories: CategoryBreakdown[];
  tags: TagBreakdown[];
};

type Raw = Record<string, unknown>;

const tagAmountOf = (value: Raw): TagAmount => ({ tag: value.tag as string, amount: value.amount as number });

const categoryOf = (value: Raw): CategoryBreakdown => ({
  id: value.id as string,
  name: value.name as string,
  color: value.color as string,
  budgetStatus: value.budgetStatus as BudgetStatus,
  budgetPercentage: value.budgetPercentage as number,
  amount: value.amount as number,
  average: value.average as number,
  tags: (value.tags as Raw[]).map(tagAmountOf),
});

const tagOf = (value: Raw): TagBreakdown => ({ tag: value.tag as string, amount: value.amount as number, average: value.average as number });

const valid = (value: Raw): Breakdown => ({
  period: value.period as Breakdown['period'],
  current: value.current as number,
  average: value.average as number,
  variation: value.variation as number,
  categories: (value.categories as Raw[]).map(categoryOf),
  tags: (value.tags as Raw[]).map(tagOf),
});

export const Breakdown = {
  valid: valid,
};
