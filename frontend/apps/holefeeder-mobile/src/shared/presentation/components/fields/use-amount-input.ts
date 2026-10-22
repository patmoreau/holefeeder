import { LocalFormatter } from '@holefeeder/shared/core';
import { useRef } from 'react';
import type { AppTextInputRef } from '@/shared/presentation/components/native/AppTextInput';
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
  const inputRef = useRef<AppTextInputRef | null>(null);
  const selectAllUntil = useRef(0);
  const formattedText = useRef(textAmount.value);

  const handleChangeText = (value: string) => {
    const { displayAmount: formatted, amount: newAmount } = formatAmount(value, currentLocale, currencyCode);
    selectAllUntil.current = 0;
    formattedText.current = formatted;
    if (formatted !== value) {
      textAmount.set(formatted);
      inputRef.current?.setSelection(formatted.length, formatted.length);
      onAmountChange(newAmount);
    }
  };

  const selectAll = () => {
    inputRef.current?.setSelection(0, textAmount.get().length);
  };

  const handleFocus = () => {
    selectAllUntil.current = Date.now() + FOCUS_SETTLE_MS;
    selectAll();
  };

  const handleBlur = () => {
    selectAllUntil.current = 0;
    inputRef.current?.setSelection(0, 0);
  };

  const handleSelectionChange = ({ start, end }: Selection) => {
    if (Date.now() > selectAllUntil.current) return;
    if (textAmount.get() !== formattedText.current) {
      selectAllUntil.current = 0;
      return;
    }
    if (start === 0 && end === textAmount.get().length) return;
    selectAllUntil.current = 0;
    selectAll();
  };

  return {
    textAmount: textAmount,
    inputRef: inputRef,
    handleChangeText: handleChangeText,
    handleFocus: handleFocus,
    handleBlur: handleBlur,
    handleSelectionChange: handleSelectionChange,
  };
};
