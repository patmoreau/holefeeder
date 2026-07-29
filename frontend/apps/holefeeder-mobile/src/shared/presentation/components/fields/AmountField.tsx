import { LocalFormatter } from '@holefeeder/shared/core';
import { useEffectEvent, useMemo } from 'react';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { useNativeState } from '@/shared/presentation/components/native/use-native-state';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useTheme } from '@/shared/theme/core/use-theme';
import { fontWeight } from '@/types/theme';

const AMOUNT_FONT_SIZE = 48;

export type AmountTone = 'negative' | 'positive' | 'neutral';

type AmountFieldProps = {
  amount: number;
  onAmountChange: (amount: number) => void;
  tone?: AmountTone;
  autoFocus?: boolean;
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

export const AmountField = ({ amount, onAmountChange, tone = 'neutral', autoFocus }: AmountFieldProps) => {
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();

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

  const amountColor = useMemo(() => {
    return tone === 'negative' ? theme.colors.negative : tone === 'positive' ? theme.colors.positive : theme.colors.text;
  }, [tone, theme.colors.negative, theme.colors.positive, theme.colors.text]);

  return (
    <AppTextInput
      value={textAmount}
      selection={selection}
      keyboardType="decimal-pad"
      onChangeText={handleChangeText}
      autoFocus={autoFocus}
      selectTextOnFocus={true}
      textStyle={{
        textAlign: 'center',
        fontSize: AMOUNT_FONT_SIZE,
        fontWeight: fontWeight.semiBold,
        color: amountColor,
      }}
    />
  );
};

AmountField.displayName = 'AmountField';
