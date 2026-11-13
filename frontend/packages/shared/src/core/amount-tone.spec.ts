import { AmountTone } from './amount-tone';

describe('AmountTone', () => {
  describe('roundsToZero', () => {
    it('is true for zero', () => {
      expect(AmountTone.roundsToZero(0)).toBe(true);
    });

    it('is true for a value under half a cent', () => {
      expect(AmountTone.roundsToZero(-0.004)).toBe(true);
    });

    it('is false for a cent', () => {
      expect(AmountTone.roundsToZero(0.01)).toBe(false);
    });
  });

  describe('of', () => {
    it('is positive for a positive value', () => {
      expect(AmountTone.of(842.17)).toBe('positive');
    });

    it('is negative for a negative value', () => {
      expect(AmountTone.of(-842.17)).toBe('negative');
    });

    it('is neutral for a value that rounds to zero', () => {
      expect(AmountTone.of(-0.001)).toBe('neutral');
    });
  });

  describe('display', () => {
    it('shows a positive value as is, with a positive tone', () => {
      expect(AmountTone.display(842.17)).toEqual({ amount: 842.17, tone: 'positive' });
    });

    it('drops the sign of a negative value and gives it a negative tone', () => {
      expect(AmountTone.display(-842.17)).toEqual({ amount: 842.17, tone: 'negative' });
    });

    it('shows zero with a neutral tone', () => {
      expect(AmountTone.display(0)).toEqual({ amount: 0, tone: 'neutral' });
    });

    it('treats a value that rounds to zero as zero', () => {
      expect(AmountTone.display(-0.001)).toEqual({ amount: 0, tone: 'neutral' });
    });
  });
});
