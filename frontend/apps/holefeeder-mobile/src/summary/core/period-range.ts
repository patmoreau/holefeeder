import { DateIntervalTypes } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';

export const PeriodRangeUnits = {
  day: 'day',
  week: 'week',
  month: 'month',
  year: 'year',
} as const;

export type PeriodRangeUnit = (typeof PeriodRangeUnits)[keyof typeof PeriodRangeUnits];

export type PeriodSpan = { unit: PeriodRangeUnit; amount: number };

const inYearsWhenWhole = (amount: number, perYear: number, unit: PeriodRangeUnit): PeriodSpan =>
  amount % perYear === 0 ? { unit: PeriodRangeUnits.year, amount: amount / perYear } : { unit, amount };

const span = (count: number, settings: Settings): PeriodSpan => {
  const amount = count * settings.frequency;

  switch (settings.intervalType) {
    case DateIntervalTypes.monthly:
      return inYearsWhenWhole(amount, 12, PeriodRangeUnits.month);
    case DateIntervalTypes.weekly:
      return inYearsWhenWhole(amount, 52, PeriodRangeUnits.week);
    case DateIntervalTypes.yearly:
      return { unit: PeriodRangeUnits.year, amount };
    default:
      return { unit: PeriodRangeUnits.day, amount };
  }
};

export const PeriodRange = {
  span: span,
};
