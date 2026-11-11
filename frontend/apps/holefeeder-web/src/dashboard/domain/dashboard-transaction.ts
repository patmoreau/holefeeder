import { DateOnly } from '@holefeeder/shared/core';

export type DashboardTransaction = {
  id: string;
  title: string;
  category: string | null;
  categoryColor: string;
  tags: string[];
  amount: number;
  date: DateOnly;
};

const valid = (value: Record<string, unknown>): DashboardTransaction => ({
  id: value.id as string,
  title: value.title as string,
  category: (value.category as string | null) ?? null,
  categoryColor: value.categoryColor as string,
  tags: value.tags as string[],
  amount: value.amount as number,
  date: DateOnly.valid(value.date),
});

const validList = (values: Record<string, unknown>[]): DashboardTransaction[] => values.map(valid);

export const DashboardTransaction = { valid: valid, validList: validList };
