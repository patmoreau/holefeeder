import { LocalFormatter } from '@holefeeder/shared/core';
import { useEffectEvent, useRef } from 'react';
import { useNativeState } from '@/shared/presentation/components/native/use-native-state';

const FOCUS_SETTLE_MS = 500;

type Selection = { start: number; end: number };

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
  const selectAllUntil = useRef(0);
  const formattedText = useRef(textAmount.value);

  const handleChangeText = useEffectEvent((value: string) => {
    // noinspection BadExpressionStatementJS
    'worklet';
    const { displayAmount: formatted, amount: newAmount } = formatAmount(value, currentLocale, currencyCode);
    selectAllUntil.current = 0;
    formattedText.current = formatted;
    if (formatted !== value) {
      textAmount.value = formatted;
      selection.value = { start: formatted.length, end: formatted.length };
      onAmountChange(newAmount);
    }
  });

  const selectAll = () => {
    selection.value = { start: 0, end: textAmount.value.length };
  };

  const handleFocus = useEffectEvent(() => {
    selectAllUntil.current = Date.now() + FOCUS_SETTLE_MS;
    selectAll();
  });

  const handleSelectionChange = useEffectEvent(({ start, end }: Selection) => {
    if (Date.now() > selectAllUntil.current) return;
    if (textAmount.value !== formattedText.current) {
      selectAllUntil.current = 0;
      return;
    }
    if (start === 0 && end === textAmount.value.length) return;
    selectAllUntil.current = 0;
    selectAll();
  });

  return {
    textAmount: textAmount,
    selection: selection,
    handleChangeText: handleChangeText,
    handleFocus: handleFocus,
    handleSelectionChange: handleSelectionChange,
  };
};
