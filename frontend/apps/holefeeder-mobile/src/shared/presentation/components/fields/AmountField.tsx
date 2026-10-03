import { useMemo } from 'react';
import { useAmountInput } from '@/shared/presentation/components/fields/use-amount-input';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
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

export const AmountField = ({ amount, onAmountChange, tone = 'neutral', autoFocus }: AmountFieldProps) => {
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const { textAmount, selection, handleChangeText } = useAmountInput({
    amount: amount,
    onAmountChange: onAmountChange,
    currentLocale: currentLocale,
    currencyCode: currencyCode,
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
