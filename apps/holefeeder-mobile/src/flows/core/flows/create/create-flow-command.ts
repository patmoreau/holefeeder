import { TagList } from '@/flows/core/flows/tag-list';
import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export type CreateFlowCommand = {
  date: DateOnly;
  amount: Money;
  description: string;
  accountId: Id;
  categoryId: Id;
  tags: TagList;
  cashflow?: { effectiveDate: DateOnly; intervalType: DateIntervalType; frequency: number; recurrence: number };
};

export const CreateFlowErrors = {
  invalidCashflowFrequency: 'invalid-cashflow-frequency',
};

const isValidFrequency = Validator.number({ min: 1 });

const create = (purchase: Record<string, unknown>): Result<CreateFlowCommand> =>
  Result.combine<CreateFlowCommand>({
    date: DateOnly.create(purchase.date),
    amount: Money.create(purchase.amount),
    description: Result.success(purchase.description as string),
    accountId: Id.create(purchase.accountId),
    categoryId: Id.create(purchase.categoryId),
    tags: TagList.create(purchase.tags),
    cashflow: createCashflow(purchase.cashflow as Record<string, unknown>),
  });

const createCashflow = (cashflow?: Record<string, unknown>): Result<CreateFlowCommand['cashflow']> => {
  if (!cashflow) return Result.success(undefined);

  type CashflowObject = NonNullable<CreateFlowCommand['cashflow']>;

  return Result.combine<CashflowObject>({
    effectiveDate: DateOnly.create(cashflow.effectiveDate),
    intervalType: Result.success(cashflow.intervalType as DateIntervalType),
    frequency: Validate.validate(isValidFrequency, cashflow.frequency, [CreateFlowErrors.invalidCashflowFrequency]),
    recurrence: Result.success(cashflow.recurrence as number),
  });
};

export const CreateFlowCommand = {
  create: create,
};
