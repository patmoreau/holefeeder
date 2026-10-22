import { DateIntervalTypes } from '@holefeeder/shared/core';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { PeriodRange } from '@/summary/core/period-range';

const settingsOf = (intervalType: (typeof DateIntervalTypes)[keyof typeof DateIntervalTypes], frequency: number) =>
  aSettings({ intervalType, frequency });

describe('PeriodRange', () => {
  describe('span', () => {
    it('spans months for one-month budgets, turning twelve into a year', () => {
      const monthly = settingsOf(DateIntervalTypes.monthly, 1);

      expect(PeriodRange.span(3, monthly)).toEqual({ unit: 'month', amount: 3 });
      expect(PeriodRange.span(6, monthly)).toEqual({ unit: 'month', amount: 6 });
      expect(PeriodRange.span(12, monthly)).toEqual({ unit: 'year', amount: 1 });
    });

    it('multiplies by the frequency for multi-month budgets', () => {
      const quarterly = settingsOf(DateIntervalTypes.monthly, 3);

      expect(PeriodRange.span(6, quarterly)).toEqual({ unit: 'month', amount: 18 });
      expect(PeriodRange.span(12, quarterly)).toEqual({ unit: 'year', amount: 3 });
    });

    it('spans weeks for weekly budgets, turning exactly 52 into a year', () => {
      expect(PeriodRange.span(6, settingsOf(DateIntervalTypes.weekly, 1))).toEqual({ unit: 'week', amount: 6 });
      expect(PeriodRange.span(6, settingsOf(DateIntervalTypes.weekly, 2))).toEqual({ unit: 'week', amount: 12 });
      expect(PeriodRange.span(26, settingsOf(DateIntervalTypes.weekly, 2))).toEqual({ unit: 'year', amount: 1 });
    });

    it('spans days and years for daily and yearly budgets', () => {
      expect(PeriodRange.span(6, settingsOf(DateIntervalTypes.daily, 1))).toEqual({ unit: 'day', amount: 6 });
      expect(PeriodRange.span(3, settingsOf(DateIntervalTypes.yearly, 1))).toEqual({ unit: 'year', amount: 3 });
    });
  });
});
