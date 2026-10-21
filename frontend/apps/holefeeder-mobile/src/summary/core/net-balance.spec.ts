import { NetBalance } from '@/summary/core/net-balance';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

const summaryOf = (totalIncome: number, currentSpending: number) => ({ ...NO_SUMMARY, totalIncome, currentSpending });

describe('NetBalance', () => {
  describe('of', () => {
    it('reports the share of income saved when income is above spending', () => {
      expect(NetBalance.of(summaryOf(4400, 3260))).toEqual({ kind: 'saved', amount: 1140, percentage: 26 });
    });

    it('reports the amount overspent when spending is above income', () => {
      expect(NetBalance.of(summaryOf(4200, 4780))).toEqual({ kind: 'overspent', amount: 580 });
    });

    it('counts all spending as overspent when there is no income', () => {
      expect(NetBalance.of(summaryOf(0, 50))).toEqual({ kind: 'overspent', amount: 50 });
    });

    it('reports all income as saved when nothing was spent', () => {
      expect(NetBalance.of(summaryOf(4400, 0))).toEqual({ kind: 'saved', amount: 4400, percentage: 100 });
    });

    it('reports nothing when there was neither income nor spending', () => {
      expect(NetBalance.of(summaryOf(0, 0))).toEqual({ kind: 'none' });
    });

    it('reports breaking even when income and spending are equal', () => {
      expect(NetBalance.of(summaryOf(1000, 1000))).toEqual({ kind: 'brokeEven' });
    });

    it('works in cents, so a one-cent surplus is saved at 0%', () => {
      expect(NetBalance.of(summaryOf(1000, 999.99))).toEqual({ kind: 'saved', amount: 0.01, percentage: 0 });
    });
  });
});
