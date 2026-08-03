import { DateIntervalType, DateOnly, Id, Money, Result, Validate, Validator } from '@holefeeder/shared/core';
import { TagList } from '@/flows/core/flows/tag-list';

export type ModifyCashflowCommand = {
  id: Id;
  effectiveDate: DateOnly;
  amount: Money;
  intervalType: DateIntervalType;
  frequency: number;
  recurrence: number;
  description: string;
  accountId: Id;
  categoryId: Id;
  tags: TagList;
};

export const ModifyCashflowErrors = {
  invalidFrequency: 'invalid-frequency',
};

const isPositiveNumber = Validator.number({ min: 1 });

const create = (value: Record<string, unknown>): Result<ModifyCashflowCommand> =>
  Result.combine<ModifyCashflowCommand>({
    id: Id.create(value.id),
    effectiveDate: DateOnly.create(value.effectiveDate),
    amount: Money.create(value.amount),
    intervalType: DateIntervalType.create(value.intervalType),
    frequency: Validate.validate(isPositiveNumber, value.frequency, [ModifyCashflowErrors.invalidFrequency]),
    recurrence: Result.success((value.recurrence as number) ?? 0),
    description: Result.success((value.description as string) ?? ''),
    accountId: Id.create(value.accountId),
    categoryId: Id.create(value.categoryId),
    tags: TagList.create(value.tags),
  });

export const ModifyCashflowCommand = {
  create: create,
};
