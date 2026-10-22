import { DateIntervalTypes, DateOnly, Money } from '@holefeeder/shared/core';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { CategoryTypes } from '@/shared/core/category-type';
import { aSummaryData } from '@/summary/__tests__/summary-data-for-test';
import { PeriodSeries } from '@/summary/core/period-series';

const d = (value: string) => DateOnly.valid(value);
const gain = (date: string, total: number) => aSummaryData({ type: CategoryTypes.gain, date: d(date), total: Money.valid(total) });
const expense = (date: string, total: number) => aSummaryData({ type: CategoryTypes.expense, date: d(date), total: Money.valid(total) });

describe('PeriodSeries', () => {
  describe('of', () => {
    const monthly = aSettings({ effectiveDate: d('2026-01-01'), intervalType: DateIntervalTypes.monthly, frequency: 1 });

    it('lists the last periods oldest first, ending with the one containing the date', () => {
      const data = [
        gain('2026-05-01', 4200),
        expense('2026-05-03', 3650),
        gain('2026-06-01', 4200),
        expense('2026-06-03', 4520),
        gain('2026-07-01', 4350),
        expense('2026-07-03', 3900),
        gain('2026-08-01', 4200),
        expense('2026-08-03', 3700),
        gain('2026-09-01', 4200),
        expense('2026-09-03', 4780),
        gain('2026-10-01', 4400),
        expense('2026-10-03', 3260),
      ];

      expect(PeriodSeries.of(data, monthly, d('2026-10-21'), 6)).toEqual([
        { from: d('2026-05-01'), to: d('2026-05-31'), gains: Money.valid(4200), expenses: Money.valid(3650) },
        { from: d('2026-06-01'), to: d('2026-06-30'), gains: Money.valid(4200), expenses: Money.valid(4520) },
        { from: d('2026-07-01'), to: d('2026-07-31'), gains: Money.valid(4350), expenses: Money.valid(3900) },
        { from: d('2026-08-01'), to: d('2026-08-31'), gains: Money.valid(4200), expenses: Money.valid(3700) },
        { from: d('2026-09-01'), to: d('2026-09-30'), gains: Money.valid(4200), expenses: Money.valid(4780) },
        { from: d('2026-10-01'), to: d('2026-10-31'), gains: Money.valid(4400), expenses: Money.valid(3260) },
      ]);
    });

    it('fills a period without transactions with zero', () => {
      const data = [gain('2026-08-01', 100), expense('2026-10-02', 50)];

      expect(PeriodSeries.of(data, monthly, d('2026-10-21'), 3)).toEqual([
        { from: d('2026-08-01'), to: d('2026-08-31'), gains: Money.valid(100), expenses: Money.ZERO },
        { from: d('2026-09-01'), to: d('2026-09-30'), gains: Money.ZERO, expenses: Money.ZERO },
        { from: d('2026-10-01'), to: d('2026-10-31'), gains: Money.ZERO, expenses: Money.valid(50) },
      ]);
    });

    it('sums several entries of a period, keeping income and expenses apart', () => {
      const data = [gain('2026-10-01', 4000), gain('2026-10-15', 400), expense('2026-10-02', 3000), expense('2026-10-20', 260)];

      expect(PeriodSeries.of(data, monthly, d('2026-10-21'), 1)).toEqual([
        { from: d('2026-10-01'), to: d('2026-10-31'), gains: Money.valid(4400), expenses: Money.valid(3260) },
      ]);
    });

    it('ignores data before the first period and after the current one', () => {
      const data = [gain('2026-08-31', 999), expense('2026-11-01', 999), gain('2026-09-01', 10)];

      expect(PeriodSeries.of(data, monthly, d('2026-10-21'), 2)).toEqual([
        { from: d('2026-09-01'), to: d('2026-09-30'), gains: Money.valid(10), expenses: Money.ZERO },
        { from: d('2026-10-01'), to: d('2026-10-31'), gains: Money.ZERO, expenses: Money.ZERO },
      ]);
    });

    it('anchors two-week periods on the budget start date', () => {
      const biweekly = aSettings({ effectiveDate: d('2026-01-02'), intervalType: DateIntervalTypes.weekly, frequency: 2 });
      const data = [gain('2026-10-01', 2100), expense('2026-10-10', 1800)];

      expect(PeriodSeries.of(data, biweekly, d('2026-10-21'), 2)).toEqual([
        { from: d('2026-09-25'), to: d('2026-10-08'), gains: Money.valid(2100), expenses: Money.ZERO },
        { from: d('2026-10-09'), to: d('2026-10-22'), gains: Money.ZERO, expenses: Money.valid(1800) },
      ]);
    });

    it('spans several months when the frequency is above one', () => {
      const quarterly = aSettings({ effectiveDate: d('2026-01-01'), intervalType: DateIntervalTypes.monthly, frequency: 3 });

      expect(PeriodSeries.of([], quarterly, d('2026-10-21'), 2)).toEqual([
        { from: d('2026-07-01'), to: d('2026-09-30'), gains: Money.ZERO, expenses: Money.ZERO },
        { from: d('2026-10-01'), to: d('2026-12-31'), gains: Money.ZERO, expenses: Money.ZERO },
      ]);
    });

    it('runs mid-month periods from the anchor day', () => {
      const midMonth = aSettings({ effectiveDate: d('2026-01-15'), intervalType: DateIntervalTypes.monthly, frequency: 1 });

      expect(PeriodSeries.of([], midMonth, d('2026-10-21'), 2)).toEqual([
        { from: d('2026-09-15'), to: d('2026-10-14'), gains: Money.ZERO, expenses: Money.ZERO },
        { from: d('2026-10-15'), to: d('2026-11-14'), gains: Money.ZERO, expenses: Money.ZERO },
      ]);
    });

    it('is empty when no period is asked for', () => {
      expect(PeriodSeries.of([gain('2026-10-01', 1)], monthly, d('2026-10-21'), 0)).toEqual([]);
      expect(PeriodSeries.of([gain('2026-10-01', 1)], monthly, d('2026-10-21'), -1)).toEqual([]);
    });

    it('adds in cents', () => {
      const data = [expense('2026-10-01', 0.1), expense('2026-10-02', 0.2)];

      expect(PeriodSeries.of(data, monthly, d('2026-10-21'), 1)).toEqual([
        { from: d('2026-10-01'), to: d('2026-10-31'), gains: Money.ZERO, expenses: Money.valid(0.3) },
      ]);
    });
  });

  const totalsOf = (gains: number, expenses: number) => ({
    from: d('2026-10-01'),
    to: d('2026-10-31'),
    gains: Money.valid(gains),
    expenses: Money.valid(expenses),
  });

  describe('totals', () => {
    it('adds income and expenses over the whole range and nets them', () => {
      expect(PeriodSeries.totals([totalsOf(4200, 4780), totalsOf(4400, 3260.1)])).toEqual({
        gains: Money.valid(8600),
        expenses: Money.valid(8040.1),
        net: 559.9,
      });
    });

    it('reports a negative net when expenses are above income', () => {
      expect(PeriodSeries.totals([totalsOf(100, 250)]).net).toBe(-150);
    });

    it('is all zero for an empty range', () => {
      expect(PeriodSeries.totals([])).toEqual({ gains: Money.ZERO, expenses: Money.ZERO, net: 0 });
    });
  });

  describe('surplusCount', () => {
    it('counts the periods where income is above expenses, not those that break even', () => {
      expect(PeriodSeries.surplusCount([totalsOf(4200, 3650), totalsOf(4200, 4520), totalsOf(1000, 1000), totalsOf(4400, 3260)])).toBe(2);
    });
  });
});
