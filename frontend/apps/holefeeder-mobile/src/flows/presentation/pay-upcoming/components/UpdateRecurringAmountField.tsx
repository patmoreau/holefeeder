import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppSwitch } from '@/shared/presentation/components/native/AppSwitch';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

type Props = {
  amount: number;
  value: boolean;
  onChange: (value: boolean) => void;
};

export const UpdateRecurringAmountField = ({ amount, value, onChange }: Props) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const formattedAmount = new Intl.NumberFormat(currentLocale, { style: 'currency', currency: currencyCode }).format(amount);

  return (
    <AppField label={t(tk.payUpcoming.updateRecurring, { amount: formattedAmount })} icon={AppIconMap.cashflow}>
      <AppSwitch value={value} onChange={onChange} />
    </AppField>
  );
};
