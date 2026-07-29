import { AmountTone } from '@/shared/presentation/components/fields/AmountField';

export const amountToneFor = (type: 'expense' | 'gain' | string): AmountTone =>
  type === 'expense' ? 'negative' : type === 'gain' ? 'positive' : 'neutral';
