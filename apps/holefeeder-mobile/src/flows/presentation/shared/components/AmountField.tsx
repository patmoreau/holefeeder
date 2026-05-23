import React, { forwardRef, useEffect, useImperativeHandle, useMemo, useRef, useState } from 'react';
import { Text, TextInput, View } from 'react-native';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const AMOUNT_FONT_SIZE = 48;

type Props = {
  amount: number;
  onAmountChange: (amount: number) => void;
  purchaseType?: PurchaseType;
};

export type AmountFieldRef = {
  focus: () => void;
};

const createStyles = (theme: Theme) => ({
  container: {
    width: '100%' as const,
  },
  row: {
    flexDirection: 'row' as const,
    alignItems: 'baseline' as const,
    justifyContent: 'center' as const,
  },
  symbol: {
    fontSize: AMOUNT_FONT_SIZE,
    fontWeight: fontWeight.semiBold,
  },
  input: {
    fontSize: AMOUNT_FONT_SIZE,
    fontWeight: fontWeight.semiBold,
    paddingVertical: spacing.lg,
    flexShrink: 1 as const,
  },
  expenseColor: { color: theme.colors.negative },
  incomeColor: { color: theme.colors.positive },
  neutralColor: { color: theme.colors.text },
});

export const AmountField = forwardRef<AmountFieldRef, Props>(({ amount, onAmountChange, purchaseType }, ref) => {
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const inputRef = useRef<TextInput>(null);

  const [displayAmount, setDisplayAmount] = useState<string>(amount.toFixed(2));
  const [enteringFocus, setEnteringFocus] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [selection, setSelection] = useState<{ start: number; end?: number } | undefined>(undefined);

  useImperativeHandle(ref, () => ({
    focus: () => {
      inputRef.current?.focus();
    },
  }));

  useEffect(() => {
    if (enteringFocus) {
      requestAnimationFrame(() => {
        setSelection({ start: 0, end: displayAmount.length });
        setEnteringFocus(false);
      });
    }
  }, [enteringFocus, displayAmount.length]);

  useEffect(() => {
    const formatted = amount.toFixed(2);
    if (formatted !== displayAmount) {
      setDisplayAmount(formatted);
    }
  }, [amount, displayAmount]);

  const currencySymbol = useMemo(() => {
    try {
      return (
        new Intl.NumberFormat(currentLocale, {
          style: 'currency',
          currency: currencyCode,
          currencyDisplay: 'narrowSymbol',
        })
          .formatToParts(0)
          .find((p) => p.type === 'currency')?.value ?? '$'
      );
    } catch {
      return '$';
    }
  }, [currentLocale, currencyCode]);

  const amountColorStyle = isEditing
    ? styles.neutralColor
    : purchaseType === PurchaseType.expense
      ? styles.expenseColor
      : purchaseType === PurchaseType.gain
        ? styles.incomeColor
        : styles.neutralColor;

  const handleChangeText = (text: string) => {
    const digits = text.replace(/\D/g, '');
    const newAmount = digits ? parseInt(digits, 10) / 100 : 0;
    setSelection(undefined);
    setDisplayAmount(newAmount.toFixed(2));
    onAmountChange(newAmount);
  };

  const handleFocus = () => {
    setIsEditing(true);
    requestAnimationFrame(() => {
      setEnteringFocus(true);
    });
  };

  const handleBlur = () => {
    setIsEditing(false);
  };

  return (
    <View style={styles.container}>
      <View style={styles.row}>
        <Text style={[styles.symbol, amountColorStyle]}>{currencySymbol}</Text>
        <TextInput
          ref={inputRef}
          style={[styles.input, amountColorStyle]}
          value={displayAmount}
          onChangeText={handleChangeText}
          onFocus={handleFocus}
          onBlur={handleBlur}
          selection={selection}
          keyboardType="decimal-pad"
        />
      </View>
    </View>
  );
});

AmountField.displayName = 'AmountField';
