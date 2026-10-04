import { displayAmount } from '@/shared/presentation/core/display-amount';

describe('displayAmount', () => {
  it('shows a positive value as is, with a positive tone', () => {
    expect(displayAmount(842.17)).toEqual({ amount: 842.17, tone: 'positive' });
  });

  it('drops the sign of a negative value and gives it a negative tone', () => {
    expect(displayAmount(-842.17)).toEqual({ amount: 842.17, tone: 'negative' });
  });

  it('shows zero with a neutral tone', () => {
    expect(displayAmount(0)).toEqual({ amount: 0, tone: 'neutral' });
  });

  it('treats a value that rounds to zero as zero', () => {
    expect(displayAmount(-0.001)).toEqual({ amount: 0, tone: 'neutral' });
  });
});
