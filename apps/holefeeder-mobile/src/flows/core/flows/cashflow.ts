import { CategoryType } from '@/flows/core/categories/category-type';
import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';
import { TagList } from './tag-list';

export type Cashflow = {
  id: Id;
  effectiveDate: DateOnly;
  amount: Money;
  intervalType: DateIntervalType;
  frequency: number;
  recurrence: number;
  description: string;
  accountId: Id;
  categoryId: Id;
  categoryType: CategoryType;
  inactive: boolean;
  tags: TagList;
};

export const CashflowErrors = {
  invalid: 'cashflow-invalid',
  neverPaid: 'cashflow-never-paid',
};

const isPositiveNumber = Validator.number({ min: 1 });
const isValidBoolean = Validator.boolean();

const create = (value: Record<string, unknown>): Result<Cashflow> =>
  Result.combine<Cashflow>({
    id: Id.create(value.id),
    effectiveDate: DateOnly.create(value.effectiveDate),
    amount: Money.create(value.amount),
    intervalType: DateIntervalType.create(value.intervalType),
    frequency: Validate.validate(isPositiveNumber, value.frequency, [CashflowErrors.invalid]),
    recurrence: Validate.validate(isPositiveNumber, value.recurrence, [CashflowErrors.invalid]),
    description: Result.success(value.description as string),
    accountId: Id.create(value.accountId),
    categoryId: Id.create(value.categoryId),
    categoryType: CategoryType.create(value.categoryType),
    inactive: Validate.validate(isValidBoolean, value.inactive, [CashflowErrors.invalid]),
    tags: TagList.create(value.tags),
  });

const valid = (value: Record<string, unknown>): Cashflow => ({
  id: Id.valid(value.id),
  effectiveDate: DateOnly.valid(value.effectiveDate),
  amount: Money.valid(value.amount),
  intervalType: DateIntervalType.valid(value.intervalType),
  frequency: value.frequency as number,
  recurrence: value.recurrence as number,
  description: value.description as string,
  accountId: Id.valid(value.accountId),
  categoryId: Id.valid(value.categoryId),
  categoryType: CategoryType.valid(value.categoryType),
  inactive: value.inactive as boolean,
  tags: TagList.valid(value.tags),
});

export const Cashflow = {
  create: create,
  valid: valid,
};
