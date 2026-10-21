import { IncomeTrend } from '@/summary/core/income-trend';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

describe('IncomeTrend', () => {
  describe('isShown', () => {
    it('hides the trend when there is no income to average', () => {
      expect(IncomeTrend.isShown({ ...NO_SUMMARY, totalIncome: 0, averageIncome: 0 })).toBe(false);
    });

    it('shows the trend when there is an average, even with nothing received yet this period', () => {
      expect(IncomeTrend.isShown({ ...NO_SUMMARY, totalIncome: 0, averageIncome: 4230 })).toBe(true);
    });

    it('treats an average that rounds to zero as no history', () => {
      expect(IncomeTrend.isShown({ ...NO_SUMMARY, averageIncome: 0.001 })).toBe(false);
    });
  });
});
