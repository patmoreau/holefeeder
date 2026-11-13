import { NetBalance } from './net-balance';

describe('NetBalance', () => {
  describe('between', () => {
    it('reports the share of income saved when income is above spending', () => {
      expect(NetBalance.between(4400, 3260)).toEqual({ kind: 'saved', amount: 1140, percentage: 26 });
    });

    it('reports the amount overspent when spending is above income', () => {
      expect(NetBalance.between(4200, 4780)).toEqual({ kind: 'overspent', amount: 580 });
    });

    it('reports spending without income as overspent', () => {
      expect(NetBalance.between(0, 50)).toEqual({ kind: 'overspent', amount: 50 });
    });

    it('reports all income saved when nothing was spent', () => {
      expect(NetBalance.between(4400, 0)).toEqual({ kind: 'saved', amount: 4400, percentage: 100 });
    });

    it('reports nothing without income or spending', () => {
      expect(NetBalance.between(0, 0)).toEqual({ kind: 'none' });
    });

    it('breaks even when income equals spending', () => {
      expect(NetBalance.between(1000, 1000)).toEqual({ kind: 'brokeEven' });
    });

    it('compares in cents, so a cent saved rounds to zero percent', () => {
      expect(NetBalance.between(1000, 999.99)).toEqual({ kind: 'saved', amount: 0.01, percentage: 0 });
    });
  });
});
