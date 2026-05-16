import React, { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';
import { TextInput, View } from 'react-native';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

type Props = {
  amount: number;
  onAmountChange: (amount: number) => void;
};

export type AmountFieldRef = {
  focus: () => void;
};

const createStyles = (theme: Theme) => ({
  container: {
    width: '100%' as const,
  },
  input: {
    color: theme.colors.secondary,
    fontSize: 48,
    fontWeight: fontWeight.semiBold,
    paddingVertical: spacing.lg,
    textAlign: 'center' as const,
  },
});

export const AmountField = forwardRef<AmountFieldRef, Props>(({ amount, onAmountChange }, ref) => {
  const { formatCurrency } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const inputRef = useRef<TextInput>(null);

  const [displayAmount, setDisplayAmount] = useState<string>(formatCurrency(amount));
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
    const formatted = formatCurrency(amount, { isEditing: isEditing });
    if (formatted !== displayAmount) {
      setDisplayAmount(formatted);
    }
  }, [amount, isEditing, displayAmount, formatCurrency]);

  const handleChangeText = (text: string) => {
    const digits = text.replace(/\D/g, '');

    const amount = digits ? parseInt(digits, 10) / 100 : 0;

    setSelection(undefined);

    setDisplayAmount(formatCurrency(amount, { isEditing: isEditing }));

    onAmountChange(amount);
  };

  const handleFocus = () => {
    setIsEditing(true);
    const newDisplayAmount = formatCurrency(amount, { isEditing: isEditing });
    setDisplayAmount(newDisplayAmount);

    requestAnimationFrame(() => {
      setEnteringFocus(true);
    });
  };

  const handleBlur = () => {
    setIsEditing(false);
  };

  return (
    <View style={[styles.container]}>
      <TextInput
        ref={inputRef}
        style={styles.input}
        value={displayAmount}
        onChangeText={handleChangeText}
        onFocus={handleFocus}
        onBlur={handleBlur}
        selection={selection}
        keyboardType="numeric"
      />
    </View>
  );
});

AmountField.displayName = 'AmountField';
