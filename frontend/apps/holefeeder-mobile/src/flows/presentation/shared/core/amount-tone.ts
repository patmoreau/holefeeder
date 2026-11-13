import { AmountTone } from '@holefeeder/shared/core';

export const amountToneFor = (type: 'expense' | 'gain' | string): AmountTone =>
  type === 'expense' ? 'negative' : type === 'gain' ? 'positive' : 'neutral';
