import { CategoryType } from '@/flows/core/categories/category-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';
import { TagList } from './tag-list';

export type Transaction = {
  id: Id;
  date: DateOnly;
  amount: Money;
  description: string;
  accountId: Id;
  categoryId: Id;
  categoryType: CategoryType;
  tags: TagList;
  cashflowId?: Id;
  cashflowDate?: DateOnly;
};

const valid = (value: Record<string, unknown>): Transaction => ({
  id: Id.valid(value.id),
  date: DateOnly.valid(value.date),
  amount: Money.valid(value.amount),
  description: value.description as string,
  accountId: Id.valid(value.accountId),
  categoryId: Id.valid(value.categoryId),
  categoryType: CategoryType.valid(value.categoryType),
  tags: TagList.valid(value.tags),
  cashflowId: value.cashflowId ? Id.valid(value.cashflowId) : undefined,
  cashflowDate: value.cashflowDate ? DateOnly.valid(value.cashflowDate) : undefined,
});

export const Transaction = { valid: valid };
