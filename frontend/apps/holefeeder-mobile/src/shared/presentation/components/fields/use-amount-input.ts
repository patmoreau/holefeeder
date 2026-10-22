import { LocalFormatter } from '@holefeeder/shared/core';
import { useRef, useState } from 'react';
import type { TextInputInstance } from 'react-native';

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
  const [text, setText] = useState(() =>
    LocalFormatter.currency(amount, currentLocale, currencyCode, {
      style: 'decimal',
    })
  );
  const inputRef = useRef<TextInputInstance | null>(null);
  const selectAllUntil = useRef(0);
  const currentText = useRef(text);

  const handleChangeText = (value: string) => {
    const { displayAmount: formatted, amount: newAmount } = formatAmount(value, currentLocale, currencyCode);
    selectAllUntil.current = 0;
    currentText.current = formatted;
    setText(formatted);
    if (formatted !== value) {
      inputRef.current?.setSelection(formatted.length, formatted.length);
      onAmountChange(newAmount);
    }
  };

  const selectAll = () => {
    inputRef.current?.setSelection(0, currentText.current.length);
  };

  const handleFocus = () => {
    selectAllUntil.current = Date.now() + FOCUS_SETTLE_MS;
    selectAll();
  };

  const handleSelectionChange = ({ start, end }: Selection) => {
    if (Date.now() > selectAllUntil.current) return;
    if (start === 0 && end === currentText.current.length) return;
    selectAllUntil.current = 0;
    selectAll();
  };

  return {
    text: text,
    inputRef: inputRef,
    handleChangeText: handleChangeText,
    handleFocus: handleFocus,
    handleSelectionChange: handleSelectionChange,
  };
};
