import { TagList } from '@/flows/core/flows/tag-list';
import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Id } from '@/shared/core/id';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { Variation } from '@/shared/core/variation';
import { CategoryType } from '../categories/category-type';

export type CashflowVariation = {
  id: Id;
  accountId: Id;
  lastPaidDate?: DateOnly;
  lastCashflowDate?: DateOnly;
  amount: Money;
  description: string;
  effectiveDate: DateOnly;
  frequency: number;
  intervalType: DateIntervalType;
  categoryType: CategoryType;
  tags: TagList;
};

export const CashflowVariationErrors = {
  neverPaid: 'cashflow-never-paid',
};

const valid = (value: Record<string, unknown>): CashflowVariation => ({
  id: Id.valid(value.id),
  accountId: Id.valid(value.accountId),
  lastPaidDate: value.lastPaidDate ? DateOnly.valid(value.lastPaidDate) : undefined,
  lastCashflowDate: value.lastCashflowDate ? DateOnly.valid(value.lastCashflowDate) : undefined,
  amount: Money.valid(value.amount),
  description: value.description as string,
  effectiveDate: DateOnly.valid(value.effectiveDate),
  frequency: value.frequency as number,
  intervalType: DateIntervalType.valid(value.intervalType),
  categoryType: CategoryType.valid(value.categoryType),
  tags: TagList.valid(value.tags),
});

const forVariations = (cashflow: CashflowVariation) => {
  const lastPaidDate = (): Result<DateOnly | undefined> => {
    if (!cashflow.lastPaidDate) return Result.failure([CashflowVariationErrors.neverPaid]);
    return Result.success(cashflow.lastPaidDate);
  };

  const lastCashflowDate = (): Result<DateOnly | undefined> => {
    if (!cashflow.lastCashflowDate) return Result.failure([CashflowVariationErrors.neverPaid]);
    return Result.success(cashflow.lastCashflowDate);
  };

  const isNotPaid = (nextDate: DateOnly): boolean => {
    // If never paid, then any date >= effectiveDate is unpaid
    if (!cashflow || !cashflow.lastPaidDate || !cashflow.lastCashflowDate) {
      return nextDate >= cashflow.effectiveDate;
    }

    return nextDate > cashflow.lastPaidDate && nextDate > cashflow.lastCashflowDate;
  };

  const getUpcomingDates = (toDate: DateOnly): DateOnly[] => {
    const dates = DateIntervalType.datesInRange(
      cashflow.effectiveDate,
      cashflow.effectiveDate,
      toDate,
      cashflow.frequency,
      cashflow.intervalType
    );
    return dates.filter((futureDate) => isNotPaid(futureDate));
  };

  const calculateUpcomingVariation = (toDate: DateOnly): Variation => {
    const upcomingDates = getUpcomingDates(toDate);

    return Variation.multiply(
      Variation.multiply(Variation.valid(cashflow.amount), CategoryType.multiplier[cashflow.categoryType]),
      upcomingDates.length
    );
  };

  return {
    lastPaidDate: lastPaidDate,
    lastCashflowDate: lastCashflowDate,
    isNotPaid: isNotPaid,
    getUpcomingDates: getUpcomingDates,
    calculateUpcomingVariation: calculateUpcomingVariation,
  };
};

export const CashflowVariation = {
  valid: valid,
  forVariation: forVariations,
};
