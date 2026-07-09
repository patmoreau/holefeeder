import { DateOnly, Id, Money, Result } from '@holefeeder/shared/core';
import { TagList } from '@/flows/core/flows/tag-list';

export type ModifyFlowCommand = {
  id: Id;
  date: DateOnly;
  amount: Money;
  description: string;
  accountId: Id;
  categoryId: Id;
  tags: TagList;
};

const create = (modify: Record<string, unknown>): Result<ModifyFlowCommand> =>
  Result.combine<ModifyFlowCommand>({
    id: Id.create(modify.id),
    date: DateOnly.create(modify.date),
    amount: Money.create(modify.amount),
    description: Result.success(modify.description as string),
    accountId: Id.create(modify.accountId),
    categoryId: Id.create(modify.categoryId),
    tags: TagList.create(modify.tags),
  });

export const ModifyFlowCommand = {
  create: create,
};
