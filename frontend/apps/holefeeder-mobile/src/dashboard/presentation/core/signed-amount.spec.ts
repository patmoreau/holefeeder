import { signedAmount } from '@/dashboard/presentation/core/signed-amount';

const NO_BREAK_SPACE = ' ';

describe('signedAmount', () => {
  it('prefixes a positive amount with a plus and a positive tone', () => {
    expect(signedAmount(true, 842.17, 'en-CA', 'CAD')).toEqual({ text: '+ $842.17', tone: 'positive' });
  });

  it('prefixes a negative amount with a minus and a negative tone', () => {
    expect(signedAmount(false, 842.17, 'en-CA', 'CAD')).toEqual({ text: '- $842.17', tone: 'negative' });
  });

  it('keeps the sign rule with the fr-CA currency format', () => {
    expect(signedAmount(true, 842.17, 'fr-CA', 'CAD')).toEqual({ text: `+ 842,17${NO_BREAK_SPACE}$`, tone: 'positive' });
  });
});
