import { LocalFormatter } from '@holefeeder/shared/core';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAmountInput } from '@/shared/presentation/components/fields/use-amount-input';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSpacer } from '@/shared/presentation/components/native/AppSpacer';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppTextInput } from '@/shared/presentation/components/native/AppTextInput';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useTheme } from '@/shared/theme/core/use-theme';
import { fontWeight } from '@/types/theme';

const AMOUNT_FONT_SIZE = 48;
const SYMBOL_FONT_SIZE = 28;

export type AmountTone = 'negative' | 'positive' | 'neutral';

export type AmountFieldProps = {
  amount: number;
  onAmountChange: (amount: number) => void;
  tone?: AmountTone;
  autoFocus?: boolean;
  testID?: string;
};

export const AmountField = ({ amount, onAmountChange, tone = 'neutral', autoFocus, testID = 'amount-field' }: AmountFieldProps) => {
  const { t } = useTranslation();
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const { textAmount, selection, handleChangeText, handleFocus, handleSelectionChange } = useAmountInput({
    amount: amount,
    onAmountChange: onAmountChange,
    currentLocale: currentLocale,
    currencyCode: currencyCode,
  });

  const amountColor = useMemo(() => {
    return tone === 'negative' ? theme.colors.negative : tone === 'positive' ? theme.colors.positive : theme.colors.text;
  }, [tone, theme.colors.negative, theme.colors.positive, theme.colors.text]);

  const currency = useMemo(() => LocalFormatter.currencySymbol(currentLocale, currencyCode), [currentLocale, currencyCode]);
  const symbol = (
    <AppText variant={'display'} textStyle={{ fontSize: SYMBOL_FONT_SIZE, fontWeight: fontWeight.semiBold, color: `${amountColor}B3` }}>
      {currency.symbol}
    </AppText>
  );

  return (
    <AppRow spacing={4} alignment={'center'}>
      <AppSpacer />
      {currency.position === 'before' && symbol}
      <AppTextInput
        value={textAmount}
        selection={selection}
        keyboardType="decimal-pad"
        onChangeText={handleChangeText}
        autoFocus={autoFocus}
        onFocus={handleFocus}
        onSelectionChange={handleSelectionChange}
        testID={testID}
        doneButton={{ label: t(tk.common.done), identifier: 'keyboard-done-button' }}
        textStyle={{
          textAlign: 'center',
          fontSize: AMOUNT_FONT_SIZE,
          fontWeight: fontWeight.semiBold,
          color: amountColor,
        }}
        modifiers={[AppModifiers.fixedSize]}
      />
      {currency.position === 'after' && symbol}
      <AppSpacer />
    </AppRow>
  );
};

AmountField.displayName = 'AmountField';
