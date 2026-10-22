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
});
