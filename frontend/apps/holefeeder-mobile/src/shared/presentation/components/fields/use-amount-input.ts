import { LocalFormatter } from '@holefeeder/shared/core';
import { useEffectEvent } from 'react';
import { useNativeState } from '@/shared/presentation/components/native/use-native-state';

type UseAmountInputProps = {
  amount: number;
  onAmountChange: (amount: number) => void;
  currentLocale: string;
  currencyCode: string;
};

const formatAmount = (input: string, currentLocale: string, currencyCode: string): { displayAmount: string; amount: number } => {
  // noinspection BadExpressionStatementJS
  'worklet';
  const digits = input.replace(/\D/g, '');
  const amount = digits ? parseInt(digits, 10) / 100 : 0;
  try {
    return {
      displayAmount: LocalFormatter.currency(amount, currentLocale, currencyCode, { style: 'decimal' }),
      amount: amount,
    };
  } catch {
    return {
      displayAmount: amount.toFixed(2),
      amount: amount,
    };
  }
};

export const useAmountInput = ({ amount, onAmountChange, currentLocale, currencyCode }: UseAmountInputProps) => {
  const textAmount = useNativeState(
    LocalFormatter.currency(amount, currentLocale, currencyCode, {
      style: 'decimal',
    })
  );
  const selection = useNativeState({ start: 0, end: 0 });

  const handleChangeText = useEffectEvent((value: string) => {
    // noinspection BadExpressionStatementJS
    'worklet';
    const { displayAmount: formatted, amount: newAmount } = formatAmount(value, currentLocale, currencyCode);
    if (formatted !== value) {
      textAmount.value = formatted;
      selection.value = { start: formatted.length, end: formatted.length };
      onAmountChange(newAmount);
    }
  });

  return {
    textAmount: textAmount,
    selection: selection,
    handleChangeText: handleChangeText,
  };
};
