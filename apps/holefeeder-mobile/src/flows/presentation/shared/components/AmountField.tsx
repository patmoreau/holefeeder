import { useNativeState } from '@expo/ui';
import { useEffectEvent, useMemo } from 'react';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { AppRow } from '@/shared/presentation/components/app/AppRow';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { AppTextInput } from '@/shared/presentation/components/app/AppTextInput';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useTheme } from '@/shared/theme/core/use-theme';
import { fontWeight } from '@/types/theme';

const AMOUNT_FONT_SIZE = 48;

type PhoneFieldProps = {
  amount: number;
  onAmountChange: (amount: number) => void;
  purchaseType?: PurchaseType;
};

const formatAmount = (input: string, currentLocale: string): { displayAmount: string; amount: number } => {
  'worklet';
  const digits = input.replace(/\D/g, '');
  const amount = digits ? parseInt(digits, 10) / 100 : 0;
  try {
    return {
      displayAmount: new Intl.NumberFormat(currentLocale, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      }).format(amount),
      amount: amount,
    };
  } catch {
    return {
      displayAmount: amount.toFixed(2),
      amount: amount,
    };
  }
};

export const PhoneField = ({ amount, onAmountChange, purchaseType }: PhoneFieldProps) => {
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();

  const textAmount = useNativeState(amount.toFixed(2));
  const selection = useNativeState({ start: 0, end: 0 });

  const handleChangeText = useEffectEvent((value: string) => {
    'worklet';
    const { displayAmount: formatted, amount: newAmount } = formatAmount(value, currentLocale);
    if (formatted !== value) {
      textAmount.value = formatted;
      selection.value = { start: formatted.length, end: formatted.length };
      onAmountChange(newAmount);
    }
  });

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

  const amountColor = useMemo(() => {
    return purchaseType === PurchaseType.expense
      ? theme.colors.negative
      : purchaseType === PurchaseType.gain
        ? theme.colors.positive
        : theme.colors.text;
  }, [purchaseType, theme.colors.negative, theme.colors.positive, theme.colors.text]);

  return (
    <AppRow alignment={'center'}>
      <AppText
        textStyle={{
          fontSize: AMOUNT_FONT_SIZE,
          fontWeight: fontWeight.semiBold,
          color: amountColor,
        }}
      >
        {currencySymbol}
      </AppText>
      <AppTextInput
        value={textAmount}
        selection={selection}
        keyboardType="decimal-pad"
        onChangeText={handleChangeText}
        selectTextOnFocus={true}
        textStyle={{
          fontSize: AMOUNT_FONT_SIZE,
          fontWeight: fontWeight.semiBold,
          color: amountColor,
        }}
      />
    </AppRow>
  );
};
