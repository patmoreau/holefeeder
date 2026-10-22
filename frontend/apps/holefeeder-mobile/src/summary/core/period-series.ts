import { DateIntervalType, DateOnly, Money } from '@holefeeder/shared/core';
import { CategoryTypes } from '@/shared/core/category-type';
import { Settings } from '@/shared/core/settings';
import { SummaryData } from './summary-data';

export type PeriodTotals = {
  from: DateOnly;
  to: DateOnly;
  gains: Money;
  expenses: Money;
};

const periodsEndingAt = (settings: Settings, asOf: DateOnly, count: number) => {
  const { effectiveDate, intervalType, frequency } = settings;
  const current = DateIntervalType.interval(effectiveDate, asOf, frequency, intervalType);

  return Array.from({ length: Math.max(0, count) }, (_, index) => {
    const stepsBack = count - 1 - index;
    const from = DateIntervalType.addIteration(current.from, -stepsBack * frequency, intervalType);
    return DateIntervalType.interval(effectiveDate, from, frequency, intervalType);
  });
};

const totalCents = (data: SummaryData[], type: string, from: DateOnly, to: DateOnly) =>
  data.filter((item) => item.type === type && item.date >= from && item.date <= to).reduce((sum, item) => sum + Money.toCents(item.total), 0);

const of = (data: SummaryData[], settings: Settings, asOf: DateOnly, count: number): PeriodTotals[] =>
  periodsEndingAt(settings, asOf, count).map(({ from, to }) => ({
    from,
    to,
    gains: Money.fromCents(totalCents(data, CategoryTypes.gain, from, to)),
    expenses: Money.fromCents(totalCents(data, CategoryTypes.expense, from, to)),
  }));

export type SeriesTotals = {
  gains: Money;
  expenses: Money;
  net: number;
};

const totals = (series: PeriodTotals[]): SeriesTotals => {
  const gainsCents = series.reduce((sum, period) => sum + Money.toCents(period.gains), 0);
  const expensesCents = series.reduce((sum, period) => sum + Money.toCents(period.expenses), 0);

  return { gains: Money.fromCents(gainsCents), expenses: Money.fromCents(expensesCents), net: (gainsCents - expensesCents) / 100 };
};

const surplusCount = (series: PeriodTotals[]): number =>
  series.filter((period) => Money.toCents(period.gains) > Money.toCents(period.expenses)).length;

export const PeriodSeries = {
  of: of,
  totals: totals,
  surplusCount: surplusCount,
};
