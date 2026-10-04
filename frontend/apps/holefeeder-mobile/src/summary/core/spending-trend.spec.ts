import { SpendingTrend } from '@/summary/core/spending-trend';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

describe('SpendingTrend', () => {
  describe('isShown', () => {
    it('hides the trend when there is no previous spending to average', () => {
      expect(SpendingTrend.isShown({ ...NO_SUMMARY, currentSpending: 42, averageSpending: 0 })).toBe(false);
    });

    it('shows the trend when there is an average, even with nothing spent yet this period', () => {
      expect(SpendingTrend.isShown({ ...NO_SUMMARY, currentSpending: 0, averageSpending: 68.52 })).toBe(true);
    });

    it('treats an average that rounds to zero as no history', () => {
      expect(SpendingTrend.isShown({ ...NO_SUMMARY, averageSpending: 0.001 })).toBe(false);
    });
  });
});
