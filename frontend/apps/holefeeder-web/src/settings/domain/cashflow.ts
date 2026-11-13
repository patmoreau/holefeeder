import { type CategoryType, type DateIntervalType, DateOnly } from '@holefeeder/shared/core';

export type Cashflow = {
  id: string;
  title: string;
  description: string;
  categoryId: string;
  categoryName: string;
  categoryColor: string;
  categoryType: CategoryType;
  accountName: string;
  amount: number;
  intervalType: DateIntervalType;
  frequency: number;
  effectiveDate: DateOnly;
  nextDate: DateOnly | null;
  tags: string[];
};

const valid = (value: Record<string, unknown>): Cashflow => ({
  id: value.id as string,
  title: value.title as string,
  description: value.description as string,
  categoryId: value.categoryId as string,
  categoryName: value.categoryName as string,
  categoryColor: value.categoryColor as string,
  categoryType: value.categoryType as CategoryType,
  accountName: value.accountName as string,
  amount: value.amount as number,
  intervalType: value.intervalType as DateIntervalType,
  frequency: value.frequency as number,
  effectiveDate: DateOnly.valid(value.effectiveDate as string),
  nextDate: value.nextDate === null ? null : DateOnly.valid(value.nextDate as string),
  tags: value.tags as string[],
});

export const Cashflow = {
  valid: valid,
};
