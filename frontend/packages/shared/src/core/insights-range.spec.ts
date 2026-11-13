import { InsightsRange } from './insights-range';

describe('InsightsRange', () => {
  it('offers 3, 6 and 12 periods', () => {
    expect(InsightsRange.options).toEqual([3, 6, 12]);
  });

  describe('restore', () => {
    it.each(['3', '6', '12'])('restores a stored %s', (stored) => {
      expect(InsightsRange.restore(stored)).toBe(Number(stored));
    });

    it('falls back to 6 when nothing is stored', () => {
      expect(InsightsRange.restore(null)).toBe(6);
    });

    it('falls back to 6 when undefined', () => {
      expect(InsightsRange.restore(undefined)).toBe(6);
    });

    it('falls back to 6 for a count that is not offered', () => {
      expect(InsightsRange.restore('4')).toBe(6);
    });

    it('falls back to 6 for text that is not a number', () => {
      expect(InsightsRange.restore('many')).toBe(6);
    });
  });
});
