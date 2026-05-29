import { useTranslation } from 'react-i18next';
import { usePayUpcomingForm } from '@/flows/presentation/pay-upcoming/core/use-pay-upcoming-form';
import { AmountField } from '@/flows/presentation/shared/components/AmountField';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/app/AppFieldSection';
import { DateField } from '@/shared/presentation/fields/DateField';

export const PayUpcomingFormContent = () => {
  const { t } = useTranslation();
  const { formData, updateFormField } = usePayUpcomingForm();

  return (
    <>
      <AmountField amount={formData.amount} onAmountChange={(amount) => updateFormField('amount', amount)} />
      <AppFieldSection>
        <DateField
          label={t(tk.purchase.basicSection.date)}
          selectedDate={formData.date}
          onDateSelected={(date) => updateFormField('date', date)}
        />
      </AppFieldSection>
    </>
  );
};
