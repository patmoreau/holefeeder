import { DateIntervalTypes, DateOnly, Money } from '@holefeeder/shared/core';
import { aSettings } from '@/shared/core/__tests__/settings-for-test';
import { PeriodLabel } from '@/summary/core/period-label';
import { PeriodTotals } from '@/summary/core/period-series';

const period = (from: string, to: string): PeriodTotals => ({
  from: DateOnly.valid(from),
  to: DateOnly.valid(to),
  gains: Money.ZERO,
  expenses: Money.ZERO,
});

describe('PeriodLabel', () => {
  describe('forSeries', () => {
    const monthly = aSettings({ intervalType: DateIntervalTypes.monthly, frequency: 1 });
    const biweekly = aSettings({ intervalType: DateIntervalTypes.weekly, frequency: 2 });
    const quarterly = aSettings({ intervalType: DateIntervalTypes.monthly, frequency: 3 });

    it('names one-month periods by their month', () => {
      const series = [period('2026-09-01', '2026-09-30'), period('2026-10-01', '2026-10-31')];

      expect(PeriodLabel.forSeries(series, monthly, 'en-CA')).toEqual(['Sep', 'Oct']);
      expect(PeriodLabel.forSeries(series, monthly, 'fr-CA')).toEqual(['sept.', 'oct.']);
    });

    it('names other periods by their first day', () => {
      const series = [period('2026-09-25', '2026-10-08'), period('2026-10-09', '2026-10-22')];

      expect(PeriodLabel.forSeries(series, biweekly, 'en-CA')).toEqual(['Sep 25', 'Oct 9']);
      expect(PeriodLabel.forSeries(series, biweekly, 'fr-CA')).toEqual(['25 sept.', '9 oct.']);
    });

    it('names multi-month periods by their first day too', () => {
      const series = [period('2026-07-01', '2026-09-30'), period('2026-10-01', '2026-12-31')];

      expect(PeriodLabel.forSeries(series, quarterly, 'en-CA')).toEqual(['Jul 1', 'Oct 1']);
    });

    it('adds the year where a series crosses into a new one', () => {
      const series = [period('2026-12-01', '2026-12-31'), period('2027-01-01', '2027-01-31'), period('2027-02-01', '2027-02-28')];

      expect(PeriodLabel.forSeries(series, monthly, 'en-CA')).toEqual(['Dec 2026', 'Jan 2027', 'Feb']);
    });
  });

  describe('forSeries with a label limit', () => {
    const monthly = aSettings({ intervalType: DateIntervalTypes.monthly, frequency: 1 });
    const months = [
      ['2025-11-01', '2025-11-30'],
      ['2025-12-01', '2025-12-31'],
      ['2026-01-01', '2026-01-31'],
      ['2026-02-01', '2026-02-28'],
      ['2026-03-01', '2026-03-31'],
      ['2026-04-01', '2026-04-30'],
      ['2026-05-01', '2026-05-31'],
      ['2026-06-01', '2026-06-30'],
      ['2026-07-01', '2026-07-31'],
      ['2026-08-01', '2026-08-31'],
      ['2026-09-01', '2026-09-30'],
      ['2026-10-01', '2026-10-31'],
    ].map(([from, to]) => period(from, to));

    it('labels every other period counting back from the current one, with the year on the first label of each year', () => {
      expect(PeriodLabel.forSeries(months, monthly, 'en-CA', 6)).toEqual([
        '',
        'Dec 2025',
        '',
        'Feb 2026',
        '',
        'Apr',
        '',
        'Jun',
        '',
        'Aug',
        '',
        'Oct',
      ]);
    });

    it('labels every period when they fit', () => {
      expect(PeriodLabel.forSeries(months.slice(6), monthly, 'en-CA', 6)).toEqual(['May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct']);
    });
  });

  describe('forTooltip', () => {
    it('names a one-month period by its month and year', () => {
      const monthly = aSettings({ intervalType: DateIntervalTypes.monthly, frequency: 1 });

      expect(PeriodLabel.forTooltip(period('2026-09-01', '2026-09-30'), monthly, 'en-CA')).toBe('September 2026');
      expect(PeriodLabel.forTooltip(period('2026-09-01', '2026-09-30'), monthly, 'fr-CA')).toBe('septembre 2026');
    });

    it('names other periods by their date range', () => {
      const biweekly = aSettings({ intervalType: DateIntervalTypes.weekly, frequency: 2 });

      expect(PeriodLabel.forTooltip(period('2026-09-25', '2026-10-08'), biweekly, 'en-CA')).toBe('Sep 25 – Oct 8');
    });
  });
});
