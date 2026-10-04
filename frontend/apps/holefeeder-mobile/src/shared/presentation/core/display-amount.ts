import { AmountTone } from '@/shared/presentation/components/fields/AmountField';

export type DisplayAmount = {
  amount: number;
  tone: AmountTone;
};

export const displayAmount = (value: number): DisplayAmount => {
  if (Math.round(Math.abs(value) * 100) === 0) return { amount: 0, tone: 'neutral' };
  return { amount: Math.abs(value), tone: value > 0 ? 'positive' : 'negative' };
};
