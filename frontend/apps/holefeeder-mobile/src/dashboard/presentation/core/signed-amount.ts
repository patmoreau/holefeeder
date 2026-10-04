import { LocalFormatter } from '@holefeeder/shared/core';
import { AmountTone } from '@/shared/presentation/components/fields/AmountField';

export type SignedAmount = {
  text: string;
  tone: AmountTone;
};

export const signedAmount = (isPositive: boolean, amount: number, locale: string, currencyCode: string): SignedAmount => {
  const formatted = LocalFormatter.currency(Math.abs(amount), locale, currencyCode);
  return isPositive ? { text: `+ ${formatted}`, tone: 'positive' } : { text: `- ${formatted}`, tone: 'negative' };
};
