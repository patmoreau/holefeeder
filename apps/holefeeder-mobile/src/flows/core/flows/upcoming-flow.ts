import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { CategoryType } from '../categories/category-type';
import { TagList } from './tag-list';
export type UpcomingFlow = {
  id: Id;
  date: DateOnly;
  amount: Money;
  description: string;
  categoryType: CategoryType;
  tags: TagList;
};

const create = (value: Record<string, unknown>): Result<UpcomingFlow> =>
  Result.combine<UpcomingFlow>({
    id: Id.create(value.id),
    date: DateOnly.create(value.date),
    amount: Money.create(value.amount),
    description: Result.success(value.description as string),
    categoryType: CategoryType.create(value.categoryType),
    tags: TagList.create(value.tags),
  });

const valid = (value: Record<string, unknown>): UpcomingFlow => ({
  id: Id.valid(value.id),
  date: DateOnly.valid(value.date),
  amount: Money.valid(value.amount),
  description: value.description as string,
  categoryType: CategoryType.valid(value.categoryType),
  tags: TagList.valid(value.tags),
});

export const UpcomingFlow = {
  create: create,
  valid: valid,
};
