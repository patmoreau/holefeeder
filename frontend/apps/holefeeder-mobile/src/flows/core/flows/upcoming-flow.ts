import { DateOnly, Id, Money, Result } from '@holefeeder/shared/core';
import { CategoryType } from '@/shared/core/category-type';
import { TagList } from './tag-list';
export type UpcomingFlow = {
  id: Id;
  date: DateOnly;
  amount: Money;
  description: string;
  categoryType: CategoryType;
  categoryName: string;
  categoryColor: string;
  tags: TagList;
};

const create = (value: Record<string, unknown>): Result<UpcomingFlow> =>
  Result.combine<UpcomingFlow>({
    id: Id.create(value.id),
    date: DateOnly.create(value.date),
    amount: Money.create(value.amount),
    description: Result.success(value.description as string),
    categoryType: CategoryType.create(value.categoryType),
    categoryName: Result.success(value.categoryName as string),
    categoryColor: Result.success(value.categoryColor as string),
    tags: TagList.create(value.tags),
  });

const valid = (value: Record<string, unknown>): UpcomingFlow => ({
  id: Id.valid(value.id),
  date: DateOnly.valid(value.date),
  amount: Money.valid(value.amount),
  description: value.description as string,
  categoryType: CategoryType.valid(value.categoryType),
  categoryName: value.categoryName as string,
  categoryColor: value.categoryColor as string,
  tags: TagList.valid(value.tags),
});

export const UpcomingFlow = {
  create: create,
  valid: valid,
};
