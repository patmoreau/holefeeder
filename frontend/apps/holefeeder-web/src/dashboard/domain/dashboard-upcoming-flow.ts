import { type CategoryType, DateOnly } from '@holefeeder/shared/core';

export type DashboardUpcomingFlow = {
  cashflowId: string;
  date: DateOnly;
  title: string;
  category: string | null;
  categoryColor: string;
  tags: string[];
  amount: number;
  categoryType: CategoryType;
};

const valid = (value: Record<string, unknown>): DashboardUpcomingFlow => ({
  cashflowId: value.cashflowId as string,
  date: DateOnly.valid(value.date),
  title: value.title as string,
  category: (value.category as string | null) ?? null,
  categoryColor: value.categoryColor as string,
  tags: value.tags as string[],
  amount: value.amount as number,
  categoryType: value.categoryType as CategoryType,
});

const validList = (values: Record<string, unknown>[]): DashboardUpcomingFlow[] => values.map(valid);

export const DashboardUpcomingFlow = { valid: valid, validList: validList };
